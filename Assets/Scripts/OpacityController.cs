// Name: Paul Lewis Marcos
// Date: February 20, 2026
// Assignment: CS 152 Project - Programming Paradigms
// Description: Dynamically generates a TMP-labeled opacity slider for each child of a
//              target parent object (e.g. MResPatient). Clicking the Opacity button
//              toggles the slider panel on and off.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpacityController : MonoBehaviour
{
    // The parent object whose children get individual opacity sliders (e.g. MResPatient)
    public GameObject targetParent;

    // Empty panel in the Canvas that will hold all generated sliders
    public GameObject opacityPanel;

    // The button that shows/hides the opacity panel
    public Button opacityButton;

    // Reference to the model interaction script to lock rotation when panel is open
    public ModelInteraction modelInteraction;

    private bool isPanelVisible = false;

    void Start()
    {
        opacityPanel.SetActive(false);
        opacityButton.onClick.AddListener(TogglePanel);

        // Add a vertical layout so sliders stack automatically
        VerticalLayoutGroup layout = opacityPanel.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
        {
            layout = opacityPanel.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        // Auto-resize the panel to fit however many sliders are built
        ContentSizeFitter fitter = opacityPanel.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = opacityPanel.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        BuildSliders();
    }

    // Toggles the opacity panel and locks/unlocks model rotation accordingly
    void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        opacityPanel.SetActive(isPanelVisible);
        if (modelInteraction != null)
            modelInteraction.canRotate = !isPanelVisible;
    }

    // Creates one labeled slider row per child of the target parent
    void BuildSliders()
    {
        if (targetParent == null) return;

        foreach (Transform child in targetParent.transform)
        {
            Renderer[] renderers = child.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0) continue;

            // Cache material instances once here — calling r.materials repeatedly
            // can return different instances and cause sliders to affect the wrong object
            List<Material> cachedMaterials = new List<Material>();
            foreach (Renderer r in renderers)
            {
                Material[] mats = r.materials;
                foreach (Material m in mats)
                {
                    PrepMaterialForTransparency(m);
                    cachedMaterials.Add(m);
                }
            }

            // --- Row container ---
            GameObject row = new GameObject(child.name + "_Row", typeof(RectTransform));
            row.transform.SetParent(opacityPanel.transform, false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0, 50);

            HorizontalLayoutGroup rowLayout = row.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 12;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = true;

            // --- TMP Label ---
            GameObject labelObj = new GameObject("Label", typeof(RectTransform));
            labelObj.transform.SetParent(row.transform, false);

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = child.name;
            label.fontSize = 14;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;

            LayoutElement labelLayout = labelObj.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 130;
            labelLayout.flexibleWidth = 0;

            // --- Slider ---
            GameObject sliderObj = BuildSlider(row.transform);
            Slider slider = sliderObj.GetComponent<Slider>();

            LayoutElement sliderLayout = sliderObj.AddComponent<LayoutElement>();
            sliderLayout.flexibleWidth = 1;

            // Bind using the cached material list so this slider always
            // targets exactly the right object's materials
            BindSlider(slider, cachedMaterials.ToArray());
        }
    }

    // Binds a slider to a pre-cached array of materials via a method parameter
    void BindSlider(Slider slider, Material[] materials)
    {
        slider.onValueChanged.AddListener(val => SetOpacity(materials, val));
    }

    // Builds a standard Unity slider entirely in code
    GameObject BuildSlider(Transform parent)
    {
        GameObject sliderObj = new GameObject("Slider", typeof(RectTransform));
        sliderObj.transform.SetParent(parent, false);
        Slider slider = sliderObj.AddComponent<Slider>();

        // Background track
        GameObject bg = new GameObject("Background", typeof(RectTransform));
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.25f);
        bgRect.anchorMax = new Vector2(1, 0.75f);
        bgRect.sizeDelta = Vector2.zero;

        // Fill area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-15, 0);

        // Fill
        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.7f, 1f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(10, 0);

        // Handle slide area
        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);

        // Handle
        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 0);

        // Wire up slider references
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        return sliderObj;
    }

    // Sets the alpha directly on the pre-cached material instances
    void SetOpacity(Material[] materials, float alpha)
    {
        foreach (Material m in materials)
        {
            Color c = m.color;
            c.a = alpha;
            m.color = c;
        }
    }

    // Switches a URP material to transparent mode so alpha changes are visible
    void PrepMaterialForTransparency(Material mat)
    {
        mat.SetFloat("_Surface", 1f);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
