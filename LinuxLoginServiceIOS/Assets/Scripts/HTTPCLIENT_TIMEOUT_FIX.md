# HttpClient Timeout Fix

## Issue

**Error Message**:
```
InvalidOperationException: This instance has already started one or more requests.
Properties can only be modified before sending the first request.
System.Net.Http.HttpClient.set_Timeout (System.TimeSpan value)
```

## Root Cause

The `MessageSender` class uses a **static HttpClient** instance (`private static readonly HttpClient _httpClient`). This is correct for performance reasons, as HttpClient is designed to be reused.

However, the code was attempting to set `_httpClient.Timeout` on **every request**:

```csharp
// PROBLEMATIC CODE (line 247)
_httpClient.Timeout = TimeSpan.FromSeconds(messageConfig.TimeoutSeconds);
```

**Why this fails**:
- `HttpClient.Timeout` is a property that can only be set **once**, before any requests are made
- Once a static HttpClient has been used for one request, its timeout cannot be changed
- Attempting to change it throws `InvalidOperationException`

## Solution

Instead of modifying `HttpClient.Timeout`, we now use **CancellationTokenSource** to control timeout on a per-request basis.

### Before (Broken):
```csharp
// Set timeout - FAILS on second request!
_httpClient.Timeout = TimeSpan.FromSeconds(messageConfig.TimeoutSeconds);

// Send request
var sendTask = _httpClient.SendAsync(request);
```

### After (Fixed):
```csharp
// Create cancellation token for timeout
using (var cts = new System.Threading.CancellationTokenSource())
{
    cts.CancelAfter(TimeSpan.FromSeconds(messageConfig.TimeoutSeconds));

    // Send request with cancellation token
    var sendTask = _httpClient.SendAsync(request, cts.Token);

    // ... handle response ...

} // CancellationTokenSource is disposed here
```

## Changes Made

**File**: `Assets/Scripts/MessageSender.cs`

**Line 247**: Removed `_httpClient.Timeout = TimeSpan.FromSeconds(messageConfig.TimeoutSeconds);`

**Lines 247-298**: Wrapped the HTTP request in a `using` statement with `CancellationTokenSource`:

```csharp
using (var cts = new System.Threading.CancellationTokenSource())
{
    cts.CancelAfter(TimeSpan.FromSeconds(messageConfig.TimeoutSeconds));
    var sendTask = _httpClient.SendAsync(request, cts.Token);

    // Wait and handle response...
}
```

## Benefits of This Approach

1. ✅ **Per-Request Timeout**: Each request can have its own timeout value
2. ✅ **Static HttpClient**: Can still use a static HttpClient for performance
3. ✅ **Proper Cancellation**: Uses .NET's standard cancellation mechanism
4. ✅ **Resource Management**: CancellationTokenSource is properly disposed via `using`
5. ✅ **No Exceptions**: No more InvalidOperationException on subsequent requests

## How It Works

1. **CancellationTokenSource** is created for each request
2. `CancelAfter()` sets the timeout duration
3. The cancellation token is passed to `SendAsync()`
4. If the request exceeds the timeout, the token triggers cancellation
5. The `using` block ensures the CancellationTokenSource is disposed after use

## Testing

This fix allows:
- Multiple requests with different timeout values
- Reusing the same MessageSender component
- Clicking Lock/Unlock buttons multiple times without errors

**Before Fix**: Second button click would throw exception
**After Fix**: All button clicks work correctly

## Technical Notes

### Why Static HttpClient?
- HttpClient is **expensive to create** (creates socket connections)
- Microsoft recommends using a **single static instance** or HttpClientFactory
- Reusing the same instance improves performance and avoids socket exhaustion

### CancellationToken vs Timeout Property
- `HttpClient.Timeout`: Global property, set once
- `CancellationToken`: Per-request mechanism, can vary
- CancellationToken is the **correct way** to handle per-request timeouts with a shared HttpClient

## Reference

Microsoft Documentation:
- [HttpClient Class](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)
- [CancellationTokenSource Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtokensource)
- [HttpClient Best Practices](https://docs.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)

## Status: ✅ Fixed

The HttpClient timeout issue has been resolved. The MessageSender now properly handles multiple requests with different timeout values using CancellationTokenSource.
