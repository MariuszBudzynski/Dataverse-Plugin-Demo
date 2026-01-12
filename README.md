A small, **educational showcase project** demonstrating how to:

* build Microsoft Dataverse / Dynamics 365 plugins in a clean, professional way
* structure plugins using a shared base class
* write **unit tests for plugins without a real Dataverse environment**
* test plugin side‑effects (Create / Update / Delete / Create related records)
* use **FakeXrmEasy (v1.x)** responsibly with **.NET Framework 4.8**

This repository is intended as a **learning resource** and a **GitHub portfolio project**.

---

## ✨ Why this project exists

Testing Dataverse plugins is often considered hard because:

* plugins run inside Dataverse, not locally
* they rely on `IOrganizationService`, `IPluginExecutionContext`, and pipeline behavior
* official tooling does not provide a local runtime

This project shows that:

> **You *can* design plugins in a testable way and validate their behavior locally.**

Even when targeting **.NET Framework 4.8**.

---

## 🧱 Project structure

```
PluginSample
│
├── PluginSample
│   ├── BasePlugin.cs
│   └── Plugins
│       ├── AccountUpdatePlugin.cs
│       └── AccountCreateNotePlugin.cs
│
├── PluginSample.FakeItTests
│   ├── Helpers
│   │   └── CreateFakeContext.cs
│   ├── AccountUpdatePluginTests.cs
│   └── AccountCreateNotePluginTests.cs
│   └── **More to come work in progress**
│
└── README.md
```

---

## 🧩 Base plugin design

All plugins inherit from a common base class:

* resolves `IOrganizationService`
* resolves `IPluginExecutionContext`
* resolves `ITracingService`
* extracts the `Target` entity (when available)

This keeps individual plugins:

* focused on business logic
* easy to read
* easy to test

```csharp
public abstract class BasePlugin : IPlugin
{
    protected IOrganizationService Service { get; private set; }
    protected IPluginExecutionContext Context { get; private set; }
    protected ITracingService Tracing { get; private set; }
    protected Entity Entity { get; private set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        Tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        var factory = (IOrganizationServiceFactory)
            serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        Service = factory.CreateOrganizationService(null);

        if (Context.InputParameters.TryGetValue("Target", out var target) && target is Entity entity)
        {
            Entity = entity;
        }

        ExecuteInternal();
    }

    protected abstract void ExecuteInternal();
}
```

## 🧪 Testing strategy

### Technology choices

* **xUnit** – test framework
* **FakeXrmEasy v1.x** – in‑memory Dataverse simulation
* **.NET Framework 4.8** – matches real Dataverse plugin runtime

> ⚠️ FakeXrmEasy v1.x is marked as deprecated, but it is **still the correct and practical choice** for .NET Framework plugin projects.

---

### Key takeaway

* **Target entity** → Arrange
* **Created / updated entities** → Assert
* Side effects are **not initialized**, only verified

---

## 🚫 What this project intentionally avoids

* real Dataverse connections
* deployment tooling
* Power Platform CLI configuration
* async plugin patterns

The goal is **clarity and testability**, not production deployment.

---

## 📌 Final notes

This project demonstrates **how plugins *should* be written**:

* minimal logic in `Execute`
* clear separation of concerns
* predictable and testable behavior

If you work with Dataverse plugins on a daily basis — this structure scales well.

---
