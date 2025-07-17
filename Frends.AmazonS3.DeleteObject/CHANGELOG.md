# Changelog

## [2.1.0] - 2025-07-17

## [2.0.0] - 2025-07-16

### Added
- New Connection tab with AWS connection parameters (AwsAccessKeyId, AwsSecretAccessKey, Region)
- ThrowErrorOnFailure option in Options tab with default value false
- ErrorMessageOnFailure option in Options tab for custom error messages
- BucketName property in SingleResultObject class
- Error class with Message and AdditionalInfo properties
- ErrorHandler helper class for standardized error handling
- Improved error reporting with separation of successful and failed operations

### Changed
- [Breaking] Moved AwsAccessKeyId, AwsSecretAccessKey, and Region from Input tab to Connection tab
- [Breaking] Moved NotExistsHandler from Options tab to Input tab and renamed to ActionOnObjectNotFound
- [Breaking] Renamed Result.Data property to Result.DeletedObjects
- [Breaking] Removed Success and Error properties from SingleResultObject class
- [Breaking] Updated Result class structure with new Error property
- Improved error handling to provide more detailed information about failed operations
- Updated parameter organization following Frends task development guidelines

### Migration Notes
- **To upgrade to version 2.0.0:**
  - Move AWS connection parameters (AwsAccessKeyId, AwsSecretAccessKey, Region) from Input tab to the new Connection tab
  - Move NotExistsHandler setting from Options tab to Input tab and rename to ActionOnObjectNotFound
  - Update any code that references Result.Data to use Result.DeletedObjects instead
  - Review error handling logic as the error structure has been updated

### Technical Changes
- Reorganized code structure following Frends development standards
- Enhanced unit test coverage for new functionality
- Updated XML documentation for all public members
- Improved separation of concerns with dedicated Connection and Error classes


## [1.0.0] - 2023-02-20
### Added
- Initial implementation 