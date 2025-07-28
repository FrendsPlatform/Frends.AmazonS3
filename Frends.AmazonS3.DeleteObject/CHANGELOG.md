# Changelog

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

## [1.0.0] - 2023-02-20
### Added
- Initial implementation 