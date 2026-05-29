# Software Architecture and Logic Flow

The application environment of the SmartPackageBox relies heavily on deeply segmented Object-Oriented principles. Developed entirely natively in C# via the `.NET 9` framework, the architecture is isolated into modular logic nodes, promoting stability and cross-platform UI synthesis via Avalonia.

![Software Flowchart](hardware/images/flowchart-soft.png)

```mermaid
mindmap
  root((SmartPackageBox))
    MainController
      Main Loop
      AI Detection
      Hatch Control
    MainFunctions
      Notification
      Picture Handling
      API Update
    AI
      Inference
      Response Handling
    MotorController
      Hatch Properties
      Move Hatch
    Api
      API Server
      Endpoints
    ImageServer
      Static File Server
    CameraFeed
      Take Picture
      Save Image
    Ntfy
      Send Notification
    NonSpecific
      Error Handling
      Logging
    FileModifyDLL
      File Operations
```

## Architectural Design and Namespaces

Rather than relying on monolithic execution scripts, the system distributes duties among several key components grouped into namespaces.

### 1. The Central Conductor: `MainController`
Established fundamentally as a Singleton, `MainController` functions stringently as the execution orchestrator across all operations. The core sequence follows strict procedural synchronization: wait for triggers, initialize frame extraction, dispatch AI HTTP requests, parse boolean returns, trigger motor mechanisms, and invoke the notification module.

### 2. Network Endpoint: `ApiHost`
Deployed as a Minimal C# API interface, `ApiHost` constructs the control bridge allowing mobile instances to access camera fields and force mechanical overrides asynchronously. 
* **Cache-Busting Integration**: Devices aggressively cache network imagery to optimize data transfers. To ensure the user's mobile app perpetually views the current drop-zone configuration, `ApiHost` algorithmically generates and appends dynamically updated timestamps (`?t=ticks`) as query parameters to image URL routings.

```mermaid
graph TD
    A[Start ApiHost] --> B{Setup WebApplicationBuilder}
    B --> B1[Clear Logging Providers]
    B1 --> B2[Set WebHost URLs]
    B2 --> C{Build WebApplication app}
    C --> D[Use HTTPS Redirection]
    D --> I{Define Endpoints}

    I --> J[Map GET /newest-url]
    J --> J1{Try}
    J1 -- Success --> J2[Return the newest imageUrl]
    
    I --> L[Map GET /update-url]
    L --> L1{apiHelper.UpdateUrl}
    L1 -- Success --> L2[Log and return Updated newest URL]

    I --> N[Map GET /move-hatch]
    N --> N1{if not hatch.isOpen}
    N1 -- True --> N3[Open Hatch]
    N1 -- False --> N5[Close Hatch]

    I --> P[Map GET /take-picture]
    P --> P2{Take Picture}
    P2 -- Success --> P3[Update Image URL]
    
    I --> T[Map GET /toggle-detection]
    T --> T1{Enable / Disable Detection}
    
    T1 --> U[app.Run]
```

### 3. Visual Buffering: `CameraFeed`
Operating the external Logitech logic presents significant buffer constraints. Under continuous testing, the camera frequently locked up or returned stale imagery out of its native internal buffer queue resulting in validation errors. 
* **Buffer-Flushing Logic**: The code initiates continuous connections but selectively pulls "dummy frames" instantly prior to executing `TakePicture()`, safely overriding the inherent camera buffer cache and generating an accurate real-time payload array.

```mermaid
graph TD
    A[Start: Handle Picture Process] --> B[Initialize Camera System]
    B --> C[Capture Photo from Camera]
    C --> D{Photo Captured Successfully?}
    D -- No --> E[Log Error: Failed to Take Picture]
    E --> F[Return: Failed]
    D -- Yes --> G[Log: Picture Taken Successfully]
    G --> I[Save Photo to File]
    I --> P[Return: Success]
    P --> Q[End: Complete]
    F --> R[End: Failed]
```

### 4. Actuation Subsystem: `MotorController`
Translating code logic back over the physical interface relies on manipulating GPIO endpoints towards the AC relay arrays. 
* **Variable Interlock**: Physical motors combust if dual-phase reverse inputs hit their capacitors. The `MotorController` software safeguards the setup by implementing a restrictive `bool isOpen` state gate which absolutely prevents executing operational commands contrary to the current physical limits regardless of API requests.

```mermaid
graph TD
    A[Start: Move Hatch] --> B{Direction Request}
    B -- Open --> C[Attempt to Open Hatch via Relay 1]
    B -- Close --> D[Attempt to Close Hatch via Relay 2]
    
    C --> E{Open Confirmed?}
    E -- Yes --> F[Update State Variable: Open]
    E -- No --> G[Log Physical Overload / Error]
    
    D --> H{Close Confirmed?}
    H -- Yes --> I[Update State Variable: Closed]
    H -- No --> J[Log Error: Obstruction]
    
    F --> K[Log Delivery State: Access Granted]
    I --> L[Log Delivery State: Secured]
```

### 5. Utilities: `Notifications` and `Logger`
* **Ntfy Interface**: Bypasses convoluted third-party messaging queues by implementing a lightweight HTTP client to a self-hosted Ntfy deployment within the homelab network. Appends visual snapshots natively as notification payloads.

```mermaid
graph TD
    A[Start: Send Notification] --> B[Generate Payload Alert String]
    B --> C[Attach Internal Base64 Image Snapshot]
    C --> D[Format External App Action Buttons]
    D --> E[POST payload to Homelab Ntfy endpoint]
    E --> F{Delivery Confirmed 200 OK?}
    F -- Yes --> G[End: Notification Success]
    F -- No --> H[Log Async Network Failure]
```

* **FileHelper**: Consolidates stack trace anomalies effectively in isolated logical chronological documents to limit crash instances to background processes.

## Software Class Relationships

The interaction array of the distinct controller dependencies and API endpoints is encapsulated functionally via the following Mermaid relational schema:

```mermaid
classDiagram
    %% AI Integration
    class AiHelper {
        - HttpClient client
        + string RoboflowLocalBaseUrl
        + string RoboflowCloudBaseUrl
        + InferenceAsync(string inferenceServerBaseUrl, string imagePath, string modelId, string modelVersion) Task~string~
    }

    class HandleResponse {
        + IsPackage(string jsonString) bool
    }

    %% Network Operations
    class ApiHost {
        + Start(HatchController hatch, string port) void
    }

    class Helper {
        <<Singleton>>
        + UpdateUrl() void
        - UpdateQueryParameter(string pNewQueryParameter) void
    }

    %% Main Application Routing
    class MainFunctions {
        <<Singleton>>
        + string apiUrl
        + handlePicture(string filePath) bool
        + trySentNotification(string pImageUrl) void
    }

    %% Hardware Integrations
    class CameraFeed {
        - VideoCapture _globalCapture
        + TakePicture() byte[]
        + ReleaseCamera() void
    }

    class HatchController {
        + MoveHatch(bool openAction) bool
        + StopHatch() bool
    }

    class Notifications {
        + sendNotification(string message, string title, string actions, string pImageUrl) Task
    }

    class ImageServer {
        + Start(string port) void
    }

    class MainController {
        + Main(string[] args) void
        + HandleAIDetection(string imagePath, bool isLocal) Task~bool~
        + packageIsDetected(MainFunctions mainFunction, HatchController hatch) void
    }

    %% Dependencies Network
    MainController ..> AiHelper : uses
    MainController ..> HandleResponse : uses
    MainController ..> MainFunctions : uses
    MainController ..> HatchController : uses
    MainController ..> ApiHost : uses
    MainController ..> ImageServer : uses
    
    ApiHost ..> HatchController : uses
    ApiHost ..> MainFunctions : uses
    ApiHost ..> Helper : uses
    
    MainFunctions ..> CameraFeed : uses
    MainFunctions ..> Notifications : uses
```

## Program Lifecycle Routing

The operational algorithm functions perpetually on state loops analyzing sensor validation against the external AI models.

```mermaid
flowchart LR
    A["USB Frame Polling initialized"] --> B{"ViT Model Validates Payload?"}
    B -- Positive validation --> C["Format payload and POST via Ntfy"]
    
    subgraph Mechanical Securization
        E["Drive GPIO Relay 1 (NC Open Phase)"]
        F["Somfy LT50 extracts hatch over drop zone"]
        G{"Avalonia companion issues release signal?"}
        H["Drive GPIO Relay 2 (NO Close Phase)"]
    end
    
    C --> E
    E --> F
    F --> G
    G -- Authorized release --> H
    B -- Negative validation --> I["Log state iteration and reset limit variables"]
```

```mermaid
flowchart LR
    subgraph App UI
        G{"Is the open-button in the app pressed?"}
        H["Open hatch to take out the package"]
    end
    
    subgraph Hardware Execution
        E["RollingHatch opens and package falls down"]
        F["Hatch closes again"]
    end
    
    A["Camera scans the internal box"] --> B{"AI evaluates: Is there a package?"}
    B -- Yes --> C["Send push-notification snapshot to Phone"]
    C --> E
    E --> F
    F --> G
    G -- Yes --> H
    B -- No --> I["No action, system waits"]
```