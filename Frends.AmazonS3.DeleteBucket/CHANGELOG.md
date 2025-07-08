# Changelog

## [1.0.0] - 2025-07-08

### Breaking Changes
- **[Breaking]** Restructured task parameters into tabbed interface with Input, Connection, and Options tabs
- **[Breaking]** Moved `BucketName` parameter from root level to `Connection` tab
- **[Breaking]** Updated result structure: removed `Data` property and added structured `Error` object
- **[Breaking]** Updated target framework to .NET 8.0

### Added
- New `Connection` tab containing:
  - `BucketName` parameter (moved from previous location)
- New `Options` tab containing:
  - `ThrowErrorOnFailure` parameter (default: true) - controls whether task throws exceptions on failure
  - `ErrorMessageOnFailure` parameter - custom error message for failures
- Enhanced error handling with structured `Error` object in result:
  - `Message` property for error descriptions
  - `AdditionalInfo` property for additional error context

### Changed
- Target framework updated from previous version to .NET 8.0
- Task now follows standardized Frends task structure with proper tab organization
- Improved error handling and reporting capabilities

### Migration Guide
To upgrade to version 2.0.0:
1. **Connection Tab**: The `BucketName` parameter has been moved to the new `Connection` tab. Update your process configuration to set the bucket name in the Connection tab instead of the previous location.
2. **Error Handling**: The task now includes standardized error handling. Review your error handling logic as the result structure has changed.
3. **Options**: New optional parameters are available in the `Options` tab for controlling error behavior. The default behavior maintains backward compatibility.

### Technical Details
- Major version bump due to breaking changes in parameter structure
- Maintains compatibility with existing AWS S3 delete bucket functionality
- Enhanced with standardized Frends task conventions for better user experience



## [1.0.0] - 2023-07-31
### Added
- Initial implementation