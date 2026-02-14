# GitHub Release Asset URL Guide

## Your Current Setup

**Release Page:** https://github.com/itsjustdel/IL2-Dials-Server/releases/tag/test-config-file

**Config File:** plane-config.json

## Stable Download URLs

GitHub provides stable, direct download URLs for release assets. There are two URL formats:

### Option 1: Latest Release (Recommended for Production) ✅

```
https://github.com/itsjustdel/IL2-Dials-Server/releases/latest/download/plane-config.json
```

**Advantages:**
- ✅ Automatically points to latest release
- ✅ Users get updates without changing client
- ✅ Perfect for production deployments
- ✅ Just publish new release and clients auto-update

**This is now configured in ConfigLoader.cs**

### Option 2: Specific Release Tag

```
https://github.com/itsjustdel/IL2-Dials-Server/releases/download/test-config-file/plane-config.json
```

**Use when:**
- Testing specific versions
- You want version control over which config clients use
- You need to maintain multiple config versions

## How to Update Your Release

### Upload plane-config.json to Release

1. **Go to your release page:**
   https://github.com/itsjustdel/IL2-Dials-Server/releases/tag/test-config-file

2. **Edit the release:**
   - Click "Edit release" button
   
3. **Attach the file:**
   - Drag and drop `plane-config.json` to the release assets area
   - Or click "Attach binaries" and select the file
   
4. **Save:**
   - Click "Update release"

### Create New Release

To publish updates:

```bash
# Tag and create release
git tag -a v1.0.0 -m "Release v1.0.0 with updated plane config"
git push origin v1.0.0
```

Then on GitHub:
1. Go to Releases → Draft a new release
2. Choose the tag you just created
3. Attach `plane-config.json`
4. Mark as latest release (important for `/latest/` URL)
5. Publish release

## Testing the URL

Test your download URL works:

```bash
# Test the URL
curl -L "https://github.com/itsjustdel/IL2-Dials-Server/releases/latest/download/plane-config.json"

# Or save to file
curl -L "https://github.com/itsjustdel/IL2-Dials-Server/releases/latest/download/plane-config.json" -o test-config.json

# Verify it's valid JSON
cat test-config.json | python -m json.tool | head
```

## URL Format Reference

General format:
```
https://github.com/{OWNER}/{REPO}/releases/download/{TAG}/{FILENAME}
```

Latest release shortcut:
```
https://github.com/{OWNER}/{REPO}/releases/latest/download/{FILENAME}
```

Your configuration:
- OWNER: `itsjustdel`
- REPO: `IL2-Dials-Server`
- TAG: `test-config-file` (or use `latest`)
- FILENAME: `plane-config.json`

## Troubleshooting

### 404 Not Found
- Check the file is attached to the release
- Verify filename matches exactly (case-sensitive)
- Ensure release is published (not draft)

### Download Fails in Client
- Test URL manually with curl first
- Check Unity console for detailed error
- Verify network connectivity
- Increase timeout if needed (ConfigUpdateManager has configurable timeout)

### Want to Use Different Release
Change in ConfigLoader.cs line 20:
```csharp
private static string _configUrl = "https://github.com/itsjustdel/IL2-Dials-Server/releases/download/YOUR-TAG/plane-config.json";
```

## Best Practices

1. **Use Semantic Versioning**
   - v1.0.0, v1.0.1, v1.1.0, etc.
   - Makes tracking changes easier

2. **Add Release Notes**
   - Document what changed in plane config
   - Users can see update history

3. **Test Before Publishing**
   - Upload to draft release first
   - Test the download URL
   - Then publish

4. **Keep Backup**
   - Old releases remain accessible
   - Can rollback by changing URL to specific tag

5. **Monitor Download Stats**
   - GitHub shows download counts
   - Can see how many users updated

## Current Configuration

The IL-2 Dials Client is now configured to use:

```
https://github.com/itsjustdel/IL2-Dials-Server/releases/latest/download/plane-config.json
```

This means:
- When you publish a new release with updated plane-config.json
- Mark it as "latest release"
- All clients will automatically download the new config when they click "Update Config"
- No client code changes needed!

## Need to Change URL?

You can change the URL at runtime:

```csharp
ConfigLoader.SetConfigUrl("https://your-new-url/plane-config.json");
```

Or permanently in code at `Assets/Scripts/Config/ConfigLoader.cs` line 20.
