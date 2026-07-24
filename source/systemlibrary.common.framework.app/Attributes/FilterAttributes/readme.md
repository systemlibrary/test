# 4-Step Header Filter System

The framework allows fencing endpoints using a header value with exact match, delimited list, wildcard, and regex.

## 1. Exact Match (Hot Path)

[ApiTokenFilter(match: "MySecretToken")]

Matches the value exactly. Fastest path.

## 2. Delimited List (|)

[UserAgentFilter(match: "Chrome|Edg|Firefox")]

Multiple allowed values. Case-insensitive substring match. Useful for User-Agent or multiple tokens.

## 3. Wildcard (*)

[ApiTokenFilter(match: "Dev*")]    // starts with "Dev"
[ApiTokenFilter(match: "*-Test")]  // ends with "-Test"
[ApiTokenFilter(match: "*Temp*")]  // contains "Temp"

Simple glob-style patterns.

## 4. Regex (Advanced / Optional)

[ApiTokenFilter(match: "^DEV-[A-Z0-9]{6}$")]

Supports any regex pattern. Only applied if the string looks like a regex. Invalid regex is ignored safely.

## Guidelines

Order: Exact -> Delimited -> Wildcard -> Regex  
Delimited and wildcard matches are case-insensitive  
Regex is optional; fallback applies if not matched  
Use delimited lists for multiple allowed values  
Use wildcards for prefix/suffix or contains matches  
Use regex for advanced patterns (length, character sets, dynamic rules)

## Examples
```csharp
[UserAgentFilter(match: "Chrome|Edg|Firefox")]  
public class CmsController : BaseApiController { ... }

[ApiTokenFilter(match: "MySecretToken")]  
public class PingController : BaseApiController { ... }

[ApiTokenFilter(match: "Client1|Client2|Client3")]

[ApiTokenFilter(match: "*.website.com")]

[ApiTokenFilter(match: "^DEV-[A-Z0-9]{6}$")]
```

## TODO
Write a comment and a sample about the BasicAuthorizationFilterAtribute to extend... or custom basic auth filtering on any controller...
- Theres a sample in the _BaseApiFilterAttribute