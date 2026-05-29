# SmartPackageBox

![Status](https://img.shields.io/badge/Status-Completed-success)
![Platform](https://img.shields.io/badge/Platform-IoT%20%7C%20.NET%20%7C%20Raspberry%20Pi-blue)
![AI](https://img.shields.io/badge/AI-ViT%20Classification-orange)

An integrated, fully automated IoT home system designed to intercept and secure delivered packages without human intervention. Built natively as a technological thesis project, the architecture merges physical relay controls on edge devices with high-level Vision Transformer (ViT) classification models and cross-platform companion software.

---

## 📖 Official Documentation

For deep technical insights regarding the structural engineering, software data-flows, AI data-labeling scripts, and Avalonia Application logic, refer strictly to the official GitHub Pages deployment:

🌐 **[Read the Full Documentation Portfolio Here](https://warreth.github.io/SmartPackageBox/)**

### Core Documentation Modules
- **[Hardware Specifications](https://warreth.github.io/SmartPackageBox/#/HARDWARE)**: Schematics on the Raspberry Pi Zero 2W, Somfy LT50 Motor, and opto-isolated 2-channel 230V relays.
- **[Software Architecture](https://warreth.github.io/SmartPackageBox/#/SOFTWARE)**: MVVM patterns, .NET 9 internal structures, buffer flushing via memory arrays, and HTTP API design.
- **[Companion Application (Avalonia UI)](https://warreth.github.io/SmartPackageBox/#/APP)**: Breakdown of the cross-platform telemetry application parsing WebAssembly, Android APKs, and external settings.
- **[Vision Transformer AI](https://warreth.github.io/SmartPackageBox/#/AI_MODEL)**: Details on the `Batch-Annotate.sh` data labeling parameters, ViT classification strategies, Base64 network tunneling payloads.

---

## System Architecture

The physical logic executes from a central localized device (`Raspberry Pi Zero 2W`) which commands automated mechanisms upon reading valid inferential data.

1. **Detect**: The system actively surveys the internal physical volume using an integrated Logitech C270 array.
2. **Classify**: Image bytes are extracted and converted to Base64, then tunneled toward an offline inference homelab executing a Binary Vision Transformer model.
3. **Actuate**: Upon receiving a valid `"Package"` boolean, the `MainController` engages the 230V AC Motor mechanical arrays, permanently sealing the compartment.
4. **Notify**: Pushes remote images via `ntfy.sh` and actively serves telemetry to the Avalonia Client dashboard.

![SmartPackageBox Hardware Impression](docs/hardware/images/Impression1.png)

## AI Dataset & Repository

The core tensor parameters are fully accessible. The ViT model structure was constructed over roughly 10,000 unique annotated image sets validating interior light curves, cardboard structures, and ambient occlusions. 

* **Roboflow Dataset**: [Package Detection Universe](https://universe.roboflow.com/eindwerk/packagedetection-xyah9)
