# Changelog

## [2.0.0] - 2026-01-07

### Added
- New Connection tab with AWS connection parameters (AwsAccessKeyId, AwsSecretAccessKey, Region)
- ThrowErrorOnFailure option in Options tab with default value true
- ErrorMessageOnFailure option in Options tab for custom error messages
- BucketName property in SingleResultObject class
- Error class with Message and AdditionalInfo properties

### Changed
- [Breaking] Moved AwsAccessKeyId, AwsSecretAccessKey, and Region from Input tab to Connection tab
- [Breaking] Moved NotExistsHandler from Options tab to Input tab and renamed to ActionOnObjectNotFound
- [Breaking] Renamed Result.Data property to Result.DeletedObjects
- [Breaking] Removed Success and Error properties from SingleResultObject class
- [Breaking] Updated Result class structure with new Error property

## [1.0.0] - 2023-02-20
### Added
- Initial implementation 