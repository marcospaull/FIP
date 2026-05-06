// Name: Paul Lewis Marcos
// Date: February 20, 2026
// Assignment: CS 152 Project - Programming Paradigms
// Description: Unity Editor window that reconstructs a 3D mesh from a DICOM medical scan
//              using SimpleITK for resampling and the Marching Cubes algorithm for surface extraction.

using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using itk.simple;
using g3;

public class Create3DModelWindow : EditorWindow
{
    // GUI input fields
    private string datasetFolder = "";
    private string modelName = "";
    private bool groupEnabled = true;
    
    // Mesh reconstruction settings
    private double numCells = 256;
    private float edgeLengthMultiplier = 2f;
    private int remeshPasses = 20;
    private int reducedTriangleCount = 10000;

    // Add menu item to open this window
    [MenuItem("CS152/Create3DModel")]
    public static void ShowWindow()
    {
        // Show the window, or focus it if already open
        EditorWindow.GetWindow(typeof(Create3DModelWindow));
    }

    // Draw the editor window UI
    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Browse a Folder"))
        {
            // Open a folder picker and grab the folder name as the model name
            datasetFolder = EditorUtility.OpenFolderPanel("Select DICOM Directory", "", "");
            modelName = Path.GetFileName(datasetFolder);
        }

        datasetFolder = EditorGUILayout.TextField("Selected Folder:", datasetFolder);
        modelName = EditorGUILayout.TextField("Model Name:", modelName);

        // Optional settings that can be toggled
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        numCells = EditorGUILayout.DoubleField("NumCells:", numCells);
        edgeLengthMultiplier = EditorGUILayout.FloatField("EdgeLengthMultiplier:", edgeLengthMultiplier);
        remeshPasses = EditorGUILayout.IntField("Smoothing Iteration:", remeshPasses);
        reducedTriangleCount = EditorGUILayout.IntField("ReducedTriangleCount:", reducedTriangleCount);
        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Reconstruct 3D Model"))
        {
            // Only run if a folder was actually selected
            if (datasetFolder.Length > 0)
            {
                ReconstructModel();
            }
        }
    }

    // Resamples the image so each voxel has 1x1x1 spacing(needed for Marching Cubes algo)
    private Image ResampleVolumeImage(Image _volumeImage)
    {
        ResampleImageFilter resample = new ResampleImageFilter();

        // For smooth resampling
        resample.SetInterpolator(InterpolatorEnum.sitkLinear);

        // Keep the same direction and origin as the original image
        resample.SetOutputDirection(_volumeImage.GetDirection());
        resample.SetOutputOrigin(_volumeImage.GetOrigin());

        // Set target spacing to 1mm in all directions
        VectorDouble newSpacing = new VectorDouble { 1.0, 1.0, 1.0 };
        resample.SetOutputSpacing(newSpacing);

        // Calculate the new dimensions based on original size and spacing
        var originalSize = _volumeImage.GetSize();
        var originalSpacing = _volumeImage.GetSpacing();
        VectorUInt32 newSize = new VectorUInt32();

        for (int i = 0; i < _volumeImage.GetDimension(); i++)
        {
            newSize.Add((uint)Math.Ceiling(originalSize[i] * originalSpacing[i] / newSpacing[i]));
        }

        resample.SetSize(newSize);

        // Filling any out of bounds areas with 0
        resample.SetDefaultPixelValue(0.0);

        // Running  the resampling
        var volumeImage = resample.Execute(_volumeImage);

        return volumeImage;
    }

    // Wraps our voxel data so Marching Cubes can sample it
    private class VolumeImplicit : BoundedImplicitFunction3d
    {
        public byte[] Data;
        public int Width, Height, Depth;

        // Define the bounding box of the volume
        public AxisAlignedBox3d Bounds()
        {
            return new AxisAlignedBox3d(0, 0, 0, Width, Height, Depth);
        }

        public double Value(ref Vector3d pt){
            int x = (int)pt.x;
            int y = (int)pt.y;
            int z = (int)pt.z;

            // Treat anything outside the volume as outside the surface
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
                return 1;

            int index = x + y * Width + z * Width * Height;
            return Data[index] > 0 ? -1 : 1;
        }
    }

    private void ReconstructModel()
    {
        Debug.Log("Starting reconstruction for: " + datasetFolder);

        // Read the DICOM series from the selected folder
        ImageSeriesReader reader = new ImageSeriesReader();
        VectorString dicomFiles = ImageSeriesReader.GetGDCMSeriesFileNames(datasetFolder);
        reader.SetFileNames(dicomFiles);
        Image volumeImage = reader.Execute();

        // Resample so voxel spacing is uniform (1x1x1)
        Image resampledImage = ResampleVolumeImage(volumeImage);

        int width = (int)resampledImage.GetSize()[0];
        int height = (int)resampledImage.GetSize()[1];
        int depth = (int)resampledImage.GetSize()[2];

        Debug.Log("Size: " + width + " x " + height + " x " + depth);

        // Copy voxel data from the image buffer into a byte array
        IntPtr buffer = resampledImage.GetBufferAsUInt8();
        byte[] pixelData = new byte[width * height * depth];
        System.Runtime.InteropServices.Marshal.Copy(buffer, pixelData, 0, pixelData.Length);

        // Set up the implicit function with our volume data
        VolumeImplicit volImplicit = new VolumeImplicit();
        volImplicit.Data = pixelData;
        volImplicit.Width = width;
        volImplicit.Height = height;
        volImplicit.Depth = depth;

        // Run Marching Cubes to extract the surface mesh
        MarchingCubes mc = new MarchingCubes();
        mc.Implicit = volImplicit;
        mc.Bounds = volImplicit.Bounds();
        mc.CubeSize = mc.Bounds.MaxDim / numCells;
        mc.IsoValue = 0;
        mc.Generate();

        DMesh3 mesh = mc.Mesh;
        Debug.Log("Vertices: " + mesh.VertexCount);

        // Smooth the mesh using remeshing passes
        Remesher remesher = new Remesher(mesh);
        remesher.SetTargetEdgeLength(edgeLengthMultiplier);
        remesher.SmoothSpeedT = 0.5;
        for (int i = 0; i < remeshPasses; i++)
        {
            remesher.BasicRemeshPass();
        }

        // Reduce polygon count for better performance
        Reducer reducer = new Reducer(mesh);
        reducer.ReduceToTriangleCount(reducedTriangleCount);

        // Compute normals, center the mesh at origin, and flip to Unity's coordinate system
        MeshNormals.QuickCompute(mesh);
        var centroid = new Vector3d(width, height, depth) / 2;
        MeshTransforms.Translate(mesh, centroid * -1);
        MeshTransforms.FlipLeftRightCoordSystems(mesh);

        // Save the finished mesh as an OBJ file in the Assets folder
        var savePath = $"{Application.dataPath}/{modelName}.obj";
        Debug.Log("Saving to: " + savePath);
        g3UnityUtils.WriteOutputMesh(mesh, savePath);

        // Refresh the asset database so the file shows up in the Project window
        AssetDatabase.Refresh();
        Debug.Log("Done!");
    }
}