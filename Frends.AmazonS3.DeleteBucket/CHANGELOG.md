# Changelog

## [2.0.0] - 2025-07-16

## [Breaking] Refactored task structure and improved error handling
- **[Breaking]** Moved `Connection.BucketName` to `Input.BucketName` - update your processes to use the Input tab for bucket name configuration
- **[Breaking]** Added new `Options` parameter class with error handling configuration
- **[Breaking]** Updated Result class structure - removed `Data` property and added `Error` property for consistent error reporting
- Added `ThrowErrorOnFailure` option (default: false) to control exception throwing behavior
- Added `ErrorMessageOnFailure` option for custom error messages
- Implemented standardized error handling using `ErrorHandler.Handle` method
- Updated project to target .NET 8.0 for improved performance and compatibility
- Enhanced XML documentation with better examples and descriptions

### Migration Guide
To upgrade to the new version:
1. Move bucket name configuration from Connection tab to Input tab
2. Configure error handling behavior in the new Options tab:
   - Set `ThrowErrorOnFailure` to `true` if you want exceptions thrown on failure (matches old behavior)
   - Set `ErrorMessageOnFailure` to provide custom error messages
3. Update any code that references the removed `Data` property from the result
4. Check the new `Error` property in the result for detailed error information when operations fail


## [1.0.0] - 2023-07-31
### Added
- Initial implementation