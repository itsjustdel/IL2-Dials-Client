# Quick Start: Attaching plane-config.json to GitHub Release

## Step-by-Step Instructions

### 1. Go to Your Release
Navigate to: https://github.com/itsjustdel/IL2-Dials-Server/releases/tag/test-config-file

### 2. Edit the Release
- Click the **"Edit release"** button (pencil icon at top right)

### 3. Attach plane-config.json
Two ways to attach:

**Option A: Drag and Drop**
- Drag `plane-config.json` file from your computer
- Drop it into the "Attach binaries by dropping them here or selecting them" area
- Wait for upload to complete

**Option B: Browse**
- Click "selecting them" link in the attachment area
- Browse to select `plane-config.json`
- Click Open

### 4. Verify Upload
- You should see `plane-config.json` listed under "Assets"
- File size should be shown (around 76KB)

### 5. Save Changes
- Click **"Update release"** button at bottom

## Testing Your URL

After attaching the file, test it works:

```bash
# Test download
curl -L "https://github.com/itsjustdel/IL2-Dials-Server/releases/download/test-config-file/plane-config.json" -o test.json

# Check it's valid JSON
cat test.json | python -m json.tool | head -20

# Check file size (should be ~76KB)
ls -lh test.json
```

Expected output:
```
-rw-r--r-- 1 user user 76K Feb 14 21:40 test.json
```

## Future Updates

When you need to update the config:

### Update the Release
1. Edit the `test-config-file` release
2. Remove old plane-config.json
3. Upload new plane-config.json
4. Save changes
5. Clients will get new config automatically!

## Troubleshooting

### "404 Not Found" when testing URL
- File not attached to release
- Filename doesn't match exactly (case-sensitive)
- Release tag doesn't match

### File won't upload
- Check file size (GitHub has 2GB limit per file)
- Check internet connection
- Try different browser

### URL works but client fails
- Check Unity console for errors
- Verify client has network access
- Check timeout setting (default 30 seconds)

## What Clients See

When users click "Update Config" in the IL-2 Dials Client:
1. Client downloads from your GitHub release URL
2. Saves to persistent storage
3. Reloads plane data
4. Shows success message

## File Location on GitHub

After upload, your file is permanently stored at:
```
https://github.com/itsjustdel/IL2-Dials-Server/releases/download/test-config-file/plane-config.json
```

## Need Help?

- Check GITHUB_RELEASE_GUIDE.md for complete documentation
- Check DEPLOYMENT_GUIDE.md for testing procedures
- Check Assets/Scripts/Config/README.md for integration details

---

**Current Status:**
✅ Client configured to use: `https://github.com/itsjustdel/IL2-Dials-Server/releases/download/test-config-file/plane-config.json`

**Next:** Attach plane-config.json to your release and test!
