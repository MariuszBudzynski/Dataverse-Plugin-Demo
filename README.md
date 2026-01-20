# Dataverse Plugin Demo

A small **showcase project** demonstrating how to:

* build Microsoft Dataverse / Dynamics 365 plugins in a clean, professional way  
* structure plugins using a shared base class  
* write **unit tests for plugins without a real Dataverse environment**  
* validate plugin side-effects (Create / Update / Delete / Related record operations)  
* use **FakeXrmEasy v1.x** responsibly with **.NET Framework 4.8**

This repository is intended as a **learning resource** and a **GitHub portfolio project**, not as a full production template.

---

## ⭐ Why this project exists

Testing Dataverse plugins is often seen as hard because:

* plugins run inside the Dataverse pipeline, not locally  
* they rely on `IOrganizationService`, `IPluginExecutionContext`, and execution context services  
* official tooling does not provide a local runtime simulation  

This project shows that:

> **You *can* design plugins in a testable way and validate behavior locally**, even when targeting **.NET Framework 4.8**.

---

## 🧱 Project Structure

```

PluginSample
│
├── PluginSample
│   ├── BasePlugin.cs
│   └── Plugins
│       ├── AccountCreateNotePlugin.cs
│       └── AccountDeletePlugin.cs
│       └── AccountUpdatePlugin.cs
│       └── AccountUpdatePrimaryContactPlugin.cs
│
├── PluginSample.FakeItTests
│   ├── Helpers
│   │   └── CreateFakeContext.cs
│   ├── AccountCreateNotePluginTests.cs
│   └── AccountDeletePluginTests.cs
│   └── AccountUpdatePluginTests.cs
│   └── AccountUpdatePrimaryContactPluginTests.cs
│
└── README.md

````

Each plugin has a corresponding test suite that verifies **side-effects** of plugin execution.

---

## 🧩 Base Plugin Design

All plugins inherit from a common base class that is responsible only for infrastructure concerns:

* resolves `IOrganizationService`  
* resolves `IPluginExecutionContext`  
* resolves `ITracingService`

The base class does **not** contain business logic nor does it automatically extract the `Target` entity.

```csharp
public abstract class BasePlugin : IPlugin
{
    protected IOrganizationService Service { get; private set; }
    protected IPluginExecutionContext Context { get; private set; }
    protected ITracingService Tracing { get; private set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        Context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));
        Tracing = (ITracingService)
            serviceProvider.GetService(typeof(ITracingService));

        var factory = (IOrganizationServiceFactory)
            serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        Service = factory.CreateOrganizationService(Context.UserId);

        ExecuteInternal();
    }

    protected abstract void ExecuteInternal();
}
````

> Handling the `Target` entity in the base class introduces hidden behavior and makes plugins harder to reason about. Leaving this to each plugin keeps behavior explicit and tests simple.

---

## 🧪 Testing Strategy

### Technology Choices

* **xUnit** – test framework
* **FakeXrmEasy v1.x** – in-memory Dataverse simulation ([GitHub][2])
* **.NET Framework 4.8** – matches real Dataverse plugin runtime

> ⚠️ FakeXrmEasy v1.x is marked as deprecated, but it remains the **correct and practical choice** for .NET Framework plugin projects.

### Focus of Tests

Plugins are tested by:

* Arranging a **target entity** and initial state
* Executing the plugin step via FakeXrmEasy
* Asserting changes in created/updated records
* Verifying side effects observable via CRM data

FakeXrmEasy does not simulate external systems, so we do *not* test integrations with actual external services.

---

## ❌ What this project intentionally avoids

* real Dataverse connections
* solution packaging and deployment (Power Platform CLI etc.)
* asynchronous plugin patterns
* external system integrations

The goal is **clarity and testability**, not production deployment completeness.

---

## 🏁 Final Notes

This project demonstrates how plugins *should* be written:

* minimal logic in `ExecuteInternal`
* clear separation of concerns
* predictable and testable behavior that scales to real-world projects
---
