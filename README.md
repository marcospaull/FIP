# 3D Medical Visualization Tool w/ Chat Bot in Unity 

**CS152 Programming Paradigms | Paul Lewis Marcos**
 | San Jose State University

---

## What is this?
This is a project for the class CS152,Programming Paradigms.
An interactive 3D medical visualization tool built in Unity. It lets users explore real patient anatomy reconstructed from CT scan data, adjust the transparency of individual organs, and ask an AI assistant questions. This application is to help student better graps the topic of anatomical structures.

---

## Features

- **3D Model Interaction** — rotate, pan, zoom, and reset anatomical models
- **Per-Organ Opacity Control** — individually fade out organs to see what's underneath
- **AI Chat Assistant** — ask anatomy questions powered by a local LLM
- **3D Reconstruction Pipeline** — reconstruct models from DICOM medical scans using Marching Cubes

---

## Anatomical Models

Reconstructed from **3D-IRCADb-01** dataset:

- Liver
- Liver Tumor 01 & 02
- Artery
- Portal Vein
- Venous System

---

## Controls

| Input | Action |
|---|---|
| Left Mouse Click + Drag | Rotate model |
| Right Mouse Click + Drag | Pan model |
| Key 9 | Zoom in |
| Key 0 | Zoom out |
| Key R | Reset model and camera |
| Opacity Button | Toggle per-organ opacity sliders |
| Hide Button | Show/hide chat UI |
| Clear Button | Cancel AI generation and clear text |

---

## Tech Stack

| Tool | Purpose |
|---|---|
| Unity 6 (6000.2.10f1) | Application framework and rendering |
| Universal Render Pipeline (URP) | Modern lighting and materials |
| LLMUnity v1.2.6 (llama.cpp) | Local LLM inference |
| Meta Llama 3.2 | AI language model |
| SimpleITK | DICOM medical image processing |
| geometry3Sharp | 3D mesh processing and optimization |
| TextMesh Pro | UI text rendering |

---

## Scripts

| Script | Description |
|---|---|
| `ModelInteraction.cs` | Handles all 3D model rotation, panning, zoom, and reset |
| `UIController.cs` | Manages the AI chat interface and hide/show toggle |
| `OpacityController.cs` | Dynamically generates per-organ opacity sliders at runtime |
| `Create3DModelWindow.cs` | Editor tool for reconstructing 3D meshes from DICOM scans |

---

## How the 3D Models Were Made

1. DICOM files loaded from the 3D-IRCADb-01 dataset
2. SimpleITK resamples and processes the volume data
3. Marching Cubes algorithm converts voxel data into a surface mesh
4. geometry3Sharp remeshes and reduces triangle count
5. Models centered and transformed into Unity's coordinate system
6. Exported as OBJ files and imported into the Unity project

---

## Dataset

**3D-IRCADb-01** — IRCAD Research
3D CT scans from
https://www.ircad.fr/research/3d-ircadb-01/

---

## References

- [3D-IRCADb-01 Dataset](https://www.ircad.fr/research/3d-ircadb-01/)
- [SimpleITK](https://simpleitk.org)
- [geometry3Sharp](https://github.com/gradientspace/geometry3Sharp)
- [Unity](https://unity.com)
- [LLMUnity](https://github.com/undreamai/LLMUnity)
- [Meta Llama 3.2](https://ai.meta.com)
