# Changelog

## [2.0.0] - 2025-07-16

- **[Breaking]** Moved `Connection.BucketName` to `Input.BucketName` - update your processes to use the Input tab for bucket name configuration
- **[Breaking]** Added new `Options` parameter class with error handling configuration
- **[Breaking]** Updated Result class structure - removed `Data` property and added `Error` property for consistent error reporting
- Added `ThrowErrorOnFailure` option (default: false) to control exception throwing behavior
- Added `ErrorMessageOnFailure` option for custom error messages
- Updated project to target .NET 8.0 for improved performance and compatibility
- Enhanced XML documentation with better examples and descriptions

## [1.0.0] - 2023-07-31
### Added
- Initial implementation