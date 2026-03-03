# NoDoze

A lightweight C# Windows utility class that prevents the system from entering sleep mode during long-running operations.

## Overview

NoDoze wraps the native Windows `SetThreadExecutionState` API to keep the system awake while your code runs. It supports both an explicit call style and a recommended `using` block style that guarantees sleep is re-enabled even if an exception is thrown.

## Requirements

- Windows
- .NET Framework 4.8

## Usage

### Recommended — `using` block (exception-safe)

Sleep is automatically re-enabled when the `using` block exits, even if an exception is thrown.

```csharp
using NoDozer;

using (NoDoze.PreventSleep())
{
    // system will not sleep during this block
    DoLongRunningWork();
}
// sleep allowed again
```

To also prevent the display from sleeping:

```csharp
using (NoDoze.PreventSleep(preventDisplaySleep: true))
{
    DoLongRunningWork();
}
```

### Explicit call style

```csharp
using NoDozer;

NoDoze.PreventSleep();
DoLongRunningWork();
NoDoze.AllowSleep();
```

> **Note:** With this style, if an exception is thrown between `PreventSleep` and `AllowSleep`, the system will remain awake indefinitely. The `using` block style is strongly preferred.

## API

### `PreventSleep(bool preventDisplaySleep = false)`

Prevents the system from sleeping. By default only system sleep is blocked; pass `true` to also block the display from sleeping.

Returns an `IDisposable` that calls `AllowSleep` when disposed.

Throws `InvalidOperationException` if the underlying API call fails.

### `AllowSleep()`

Restores the previous thread execution state, re-enabling normal sleep behavior.

Throws `InvalidOperationException` if the underlying API call fails.

## How It Works

NoDoze calls the Windows [`SetThreadExecutionState`](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate) API with the appropriate flags:

| Flag | Purpose |
|---|---|
| `ES_CONTINUOUS` | Keep the state set until the next call |
| `ES_SYSTEM_REQUIRED` | Prevent system sleep |
| `ES_DISPLAY_REQUIRED` | Prevent display sleep (optional) |

The previous execution state is saved when `PreventSleep` is called and restored when `AllowSleep` is called.

