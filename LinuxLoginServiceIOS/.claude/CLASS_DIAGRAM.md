# MessageSender System - Class Diagram

## UML Class Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                        LinuxLoginService.Models                      │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌────────────────────────────────┐    ┌──────────────────────────┐ │
│  │    ConnectionConfig            │    │    MessageConfig         │ │
│  ├────────────────────────────────┤    ├──────────────────────────┤ │
│  │ - _targetIp: string            │    │ - _payload: string       │ │
│  │ - _port: int                   │    │ - _timeoutSeconds: float │ │
│  │ - _apiKey: string              │    │ - _contentType: string   │ │
│  │ - _useHttps: bool              │    │ - _httpMethod: enum      │ │
│  │ - _endpoint: string            │    ├──────────────────────────┤ │
│  ├────────────────────────────────┤    │ + Payload: string        │ │
│  │ + TargetIp: string             │    │ + TimeoutSeconds: float  │ │
│  │ + Port: int                    │    │ + ContentType: string    │ │
│  │ + ApiKey: string               │    │ + Method: HttpMethod     │ │
│  │ + UseHttps: bool               │    ├──────────────────────────┤ │
│  │ + Endpoint: string             │    │ + IsValid(): bool        │ │
│  │ + Protocol: string             │    │ + Clone(): MessageConfig │ │
│  ├────────────────────────────────┤    │ + CopyFrom(other): void  │ │
│  │ + ConstructUrl(): string       │    │ + ToString(): string     │ │
│  │ + IsValid(): bool              │    └──────────────────────────┘ │
│  │ + Clone(): ConnectionConfig    │                                 │
│  │ + CopyFrom(other): void        │    ┌──────────────────────────┐ │
│  │ + ToString(): string           │    │ HttpMethod (enum)        │ │
│  └────────────────────────────────┘    ├──────────────────────────┤ │
│                                         │ GET                      │ │
│                                         │ POST                     │ │
│                                         │ PUT                      │ │
│                                         │ DELETE                   │ │
│                                         │ PATCH                    │ │
│                                         └──────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────┐
│                     LinuxLoginService.Services                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  ┌────────────────────────────────────────────────────────────────┐ │
│  │                    MessageSender : MonoBehaviour               │ │
│  ├────────────────────────────────────────────────────────────────┤ │
│  │ - _connectionConfig: ConnectionConfig                          │ │
│  │ - _messageConfig: MessageConfig                                │ │
│  │ - _isSending: bool                                             │ │
│  │ - static _httpClient: HttpClient                               │ │
│  ├────────────────────────────────────────────────────────────────┤ │
│  │ + Connection: ConnectionConfig                                 │ │
│  │ + Message: MessageConfig                                       │ │
│  │ + IsSending: bool                                              │ │
│  │ + FullUrl: string                                              │ │
│  ├────────────────────────────────────────────────────────────────┤ │
│  │ + OnMessageSent: UnityEvent<string>                            │ │
│  │ + OnMessageFailed: UnityEvent<string>                          │ │
│  ├────────────────────────────────────────────────────────────────┤ │
│  │ + SendMessage(): void                                          │ │
│  │ + SendMessage(ConnectionConfig): void                          │ │
│  │ + SendMessage(ConnectionConfig, MessageConfig): void           │ │
│  │ + SendMessageWithParams(ip, port, key): void                   │ │
│  │ + SendCustomMessage(payload): void                             │ │
│  │ + UpdateConnectionConfig(config): void                         │ │
│  │ + UpdateConnectionParams(ip, port, key): void                  │ │
│  │ + UpdateMessageConfig(config): void                            │ │
│  ├────────────────────────────────────────────────────────────────┤ │
│  │ - SendMessageCoroutine(conn, msg): IEnumerator                 │ │
│  │ - CreateHttpRequest(conn, msg): HttpRequestMessage             │ │
│  │ - OnValidate(): void                                           │ │
│  └────────────────────────────────────────────────────────────────┘ │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘
```

## Composition Relationship

```
╔═══════════════════════════════════════════════════════════════╗
║                         MessageSender                          ║
║                      (Service/Component)                       ║
╠═══════════════════════════════════════════════════════════════╣
║                                                                ║
║   ┌────────────────────┐          ┌────────────────────┐     ║
║   │ ConnectionConfig   │◆────────◆│  MessageConfig     │     ║
║   │  (Composition)     │          │   (Composition)    │     ║
║   └────────────────────┘          └────────────────────┘     ║
║           │                                  │                ║
║           │                                  │                ║
║           ├─ TargetIp                        ├─ Payload      ║
║           ├─ Port                            ├─ Timeout      ║
║           ├─ ApiKey                          ├─ ContentType  ║
║           ├─ UseHttps                        └─ HttpMethod   ║
║           └─ Endpoint                                        ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
```

## Object Interaction Flow

```
┌──────────┐                 ┌───────────────┐
│  Button  │                 │ MessageSender │
└────┬─────┘                 └───────┬───────┘
     │                               │
     │ OnClick()                     │
     ├──────────────────────────────►│
     │                               │
     │                               │ Validate Configs
     │                               ├──────────┐
     │                               │          │
     │                               │◄─────────┘
     │                               │
     │                               │ CreateHttpRequest()
     │                               ├────────────────────┐
     │                               │                    │
     │                               │ Uses:              │
     │                               │ - ConnectionConfig │
     │                               │ - MessageConfig    │
     │                               │◄───────────────────┘
     │                               │
     │                               │ SendAsync()
     │                               ├──────────────────►[HTTP Server]
     │                               │                         │
     │                               │         Response        │
     │                               │◄────────────────────────┤
     │                               │                         │
     │                               │ OnMessageSent.Invoke()  │
     │   Event Callback              │◄────────────────────────┘
     │◄──────────────────────────────┤
     │                               │
```

## Usage Pattern Examples

### Pattern 1: Direct Configuration Access

```
┌─────────────────────┐
│  Your Script        │
└──────┬──────────────┘
       │
       │ messageSender.Connection.TargetIp = "192.168.1.100"
       ├────────────────────────────────────────────────────┐
       │                                                     │
       ▼                                                     ▼
┌──────────────────┐                              ┌──────────────────┐
│ MessageSender    │      Has-A                   │ ConnectionConfig │
│                  │◆───────────────────────────► │                  │
│                  │                               │ TargetIp = "..." │
└──────────────────┘                              └──────────────────┘
```

### Pattern 2: Configuration Object Passing

```
┌─────────────────────┐
│  Your Script        │
└──────┬──────────────┘
       │
       │ Create new ConnectionConfig(...)
       ├────────────────────────────┐
       │                             ▼
       │                    ┌──────────────────┐
       │                    │ ConnectionConfig │
       │                    │  (Temporary)     │
       │                    └────────┬─────────┘
       │                             │
       │ SendMessage(config)         │
       ├─────────────────────────────┼─────────┐
       │                             │         │
       ▼                             ▼         ▼
┌──────────────────┐        ┌──────────────────┐
│ MessageSender    │  Uses  │ ConnectionConfig │
│                  │───────►│   (Parameter)    │
└──────────────────┘        └──────────────────┘
```

### Pattern 3: Configuration Presets

```
┌─────────────────────────────────────────────────────┐
│  Your Script (Inspector Serialized)                 │
├─────────────────────────────────────────────────────┤
│                                                      │
│  [SerializeField] ConnectionConfig _devConfig       │
│  [SerializeField] ConnectionConfig _prodConfig      │
│                                                      │
└──────┬───────────────────────────┬──────────────────┘
       │                           │
       │ SwitchToDev()             │ SwitchToProd()
       │                           │
       ▼                           ▼
┌──────────────────┐        ┌──────────────────┐
│ ConnectionConfig │        │ ConnectionConfig │
│  (Dev Preset)    │        │  (Prod Preset)   │
└────────┬─────────┘        └────────┬─────────┘
         │                           │
         │ SendMessage(config)       │ SendMessage(config)
         └───────────┬───────────────┘
                     │
                     ▼
            ┌──────────────────┐
            │ MessageSender    │
            └──────────────────┘
```

## Class Relationships Summary

```
┌─────────────────────────────────────────────────────────────────┐
│                      Relationship Types                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  MessageSender  ◆────────► ConnectionConfig  (Composition)      │
│  MessageSender  ◆────────► MessageConfig     (Composition)      │
│  MessageConfig  ◇────────► HttpMethod        (Aggregation)      │
│  MessageSender  ────────►  HttpClient        (Association)      │
│  MessageSender  ────────►  UnityEvent<>      (Association)      │
│  MonoBehaviour  ◄────────  MessageSender     (Inheritance)      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

Legend:
  ◆  Composition (strong ownership)
  ◇  Aggregation (weak ownership)
  ──► Association (uses)
  ◄── Inheritance (is-a)
```

## Dependency Graph

```
                    ┌──────────────────┐
                    │  UnityEngine     │
                    │  MonoBehaviour   │
                    └────────┬─────────┘
                             │
                             │ Inherits
                             │
                             ▼
                    ┌──────────────────┐
          ┌─────────┤ MessageSender    ├─────────┐
          │         └──────────────────┘         │
          │                                      │
          │ Uses                           Uses  │
          │                                      │
          ▼                                      ▼
┌──────────────────┐                  ┌──────────────────┐
│ ConnectionConfig │                  │ MessageConfig    │
├──────────────────┤                  ├──────────────────┤
│ - TargetIp       │                  │ - Payload        │
│ - Port           │                  │ - TimeoutSeconds │
│ - ApiKey         │                  │ - ContentType    │
│ - UseHttps       │                  │ - Method         │
│ - Endpoint       │                  └────────┬─────────┘
└──────────────────┘                           │
                                               │ Uses
                                               │
                                               ▼
                                     ┌──────────────────┐
                                     │ HttpMethod (enum)│
                                     ├──────────────────┤
                                     │ GET, POST,       │
                                     │ PUT, DELETE,     │
                                     │ PATCH            │
                                     └──────────────────┘
```

## Data Flow Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│                         User Input Layer                         │
├──────────────────────────────────────────────────────────────────┤
│  ┌──────┐  ┌──────┐  ┌──────┐  ┌──────┐                        │
│  │Button│  │Button│  │Button│  │Button│                        │
│  │  1   │  │  2   │  │  3   │  │  4   │                        │
│  └───┬──┘  └───┬──┘  └───┬──┘  └───┬──┘                        │
└──────┼─────────┼─────────┼─────────┼───────────────────────────┘
       │         │         │         │
       │         │         │         │ Each button triggers
       │         │         │         │ with different config
       │         │         │         │
       └─────────┴─────────┴─────────┴───────────┐
                                                  │
┌─────────────────────────────────────────────────┼───────────────┐
│                   Service Layer                 │               │
├─────────────────────────────────────────────────┼───────────────┤
│                                                  ▼               │
│                                       ┌──────────────────┐      │
│                                       │ MessageSender    │      │
│                                       ├──────────────────┤      │
│                                       │ - Validate       │      │
│                                       │ - CreateRequest  │      │
│                                       │ - Send           │      │
│                                       │ - HandleResponse │      │
│                                       └────────┬─────────┘      │
└────────────────────────────────────────────────┼────────────────┘
                                                  │
                                                  │
┌─────────────────────────────────────────────────┼───────────────┐
│                    Model Layer                  │               │
├─────────────────────────────────────────────────┼───────────────┤
│                                                  │               │
│                           ┌──────────────────────┴────────┐     │
│                           │                               │     │
│                           ▼                               ▼     │
│                 ┌──────────────────┐          ┌──────────────┐ │
│                 │ ConnectionConfig │          │MessageConfig │ │
│                 ├──────────────────┤          ├──────────────┤ │
│                 │ IP, Port, Key    │          │Payload, Time │ │
│                 └──────────────────┘          └──────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                    External Communication                        │
├─────────────────────────────────────────────────────────────────┤
│                        HTTP Request                              │
│                             │                                    │
│                             ▼                                    │
│                    [Target Server]                               │
│                             │                                    │
│                    HTTP Response                                 │
│                             │                                    │
│                             ▼                                    │
│                   ┌────────────────┐                            │
│                   │ Events         │                            │
│                   ├────────────────┤                            │
│                   │ OnMessageSent  │                            │
│                   │ OnMessageFailed│                            │
│                   └────────────────┘                            │
└─────────────────────────────────────────────────────────────────┘
```

## Sequence Diagram - Send Message Flow

```
UI Button    MessageSender    ConnectionConfig    MessageConfig    HTTP Server
   │               │                  │                 │               │
   │  onClick()    │                  │                 │               │
   ├──────────────►│                  │                 │               │
   │               │                  │                 │               │
   │               │ IsValid()?       │                 │               │
   │               ├─────────────────►│                 │               │
   │               │  ◄───────────────┤                 │               │
   │               │     true         │                 │               │
   │               │                  │                 │               │
   │               │                  │  IsValid()?     │               │
   │               ├──────────────────┼────────────────►│               │
   │               │                  │  ◄──────────────┤               │
   │               │                  │      true       │               │
   │               │                  │                 │               │
   │               │ ConstructUrl()   │                 │               │
   │               ├─────────────────►│                 │               │
   │               │  ◄───────────────┤                 │               │
   │               │    url string    │                 │               │
   │               │                  │                 │               │
   │               │ CreateHttpRequest(ConnectionConfig, MessageConfig) │
   │               ├────────────────────────────────────┐               │
   │               │  (builds HTTP request)             │               │
   │               │◄───────────────────────────────────┘               │
   │               │                  │                 │               │
   │               │                POST Request        │               │
   │               ├────────────────────────────────────┼──────────────►│
   │               │                  │                 │               │
   │               │                  │                 │ Processing... │
   │               │                  │                 │               │
   │               │                  │        Response │               │
   │               │◄────────────────────────────────────────────────────
   │               │                  │                 │               │
   │               │ OnMessageSent.Invoke(response)     │               │
   │◄──────────────┤                  │                 │               │
   │ Event Handled │                  │                 │               │
```

## File Organization Tree

```
Assets/Scripts/
│
├── Models/                          (Data Models - Reusable)
│   ├── ConnectionConfig.cs          ├─ IP, Port, ApiKey, Endpoint
│   │   ├── Properties               │  ├─ Validation
│   │   ├── Methods                  │  ├─ URL Construction
│   │   └── Constructors             │  └─ Cloning
│   │
│   └── MessageConfig.cs             ├─ Payload, Timeout, Method
│       ├── Properties               │  ├─ Validation
│       ├── Methods                  │  ├─ Cloning
│       ├── Constructors             │  └─ ContentType
│       └── HttpMethod (enum)        └─ GET, POST, PUT, DELETE, PATCH
│
├── Services/                        (Business Logic)
│   └── MessageSender.cs             ├─ Service Component
│       ├── Fields                   │  ├─ ConnectionConfig (composition)
│       ├── Properties               │  ├─ MessageConfig (composition)
│       ├── Events                   │  ├─ OnMessageSent
│       ├── Public Methods           │  ├─ OnMessageFailed
│       │   ├── SendMessage()        │  └─ HttpClient (static)
│       │   ├── SendMessage(conn)
│       │   ├── SendMessage(conn, msg)
│       │   ├── SendMessageWithParams()
│       │   ├── SendCustomMessage()
│       │   ├── UpdateConnectionConfig()
│       │   └── UpdateMessageConfig()
│       │
│       └── Private Methods
│           ├── SendMessageCoroutine()
│           ├── CreateHttpRequest()
│           └── OnValidate()
│
└── Examples/                        (Usage Examples)
    └── MessageSenderExample.cs      └─ Demonstrates all usage patterns
```

---

## Architecture Principles Applied

### SOLID Principles

**S - Single Responsibility**
- `ConnectionConfig`: Only handles connection parameters
- `MessageConfig`: Only handles message parameters
- `MessageSender`: Only handles HTTP communication

**O - Open/Closed**
- Easy to extend with new config types
- New HTTP methods can be added to enum
- Closed for modification of existing behavior

**L - Liskov Substitution**
- Model classes are interchangeable
- Any valid `ConnectionConfig` works with any `MessageSender`

**I - Interface Segregation**
- Models have focused, minimal APIs
- No forced dependencies on unused methods

**D - Dependency Inversion**
- `MessageSender` depends on abstractions (model classes)
- Not directly dependent on specific implementations

---

This diagram shows the complete architecture of the refactored OOP system!
