# Changelog

## [2.0.0] - 2025-07-09

### Changed
- **[Breaking]** Moved `BucketName` parameter from `Connection` tab to new `Input` tab
  - Migration: Update your process configuration to set the bucket name in the Input tab instead of the previous location.
- **[Breaking]** Updated target framework to .NET 8.0
  - Migration: Update to Frends with .NET 8.0 support
- New `Options` tab containing:
  - `ThrowErrorOnFailure` parameter (default: true) - controls whether task throws exceptions on failure
  - `ErrorMessageOnFailure` parameter - custom error message for failures
- Task now follows standardized Frends task structure with proper tab organization
- Improved error handling and reporting capabilities
- Enhanced error handling with structured `Error` object in result:
  - `Message` property for error descriptions
  - `AdditionalInfo` property for additional error context
- **[Breaking]** Removed deprecated `Data` parameter from `Result` object
  - Migration: Update your process to be aware of the removal of the `Data` parameter from the `Result` object.



## [1.0.0] - 2023-07-31
### Added
- Initial implementation