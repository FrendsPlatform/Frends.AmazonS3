# Changelog

## [3.0.0] - 2025-07-17

## [Breaking] Major refactoring - restructured parameters into Input, Connection, and Options classes with improved error handling

### Breaking Changes
- **Parameter Structure Reorganization**: Task parameters have been restructured into three main classes:
  - **Input**: Contains core task parameters (BucketName, SourceDirectory, SearchPattern, DownloadFromCurrentDirectoryOnly, TargetDirectory)
  - **Connection**: Contains connection-related parameters (AwsCredentials, PreSignedUrl)
  - **Options**: Contains optional configuration parameters (FileLockedRetries, DeleteSourceObject, ThrowErrorIfNoMatch, ActionOnExistingFile, ThrowErrorOnFailure, ErrorMessageOnFailure)

### Parameter Changes
- **Moved to Input tab**:
  - `Connection.BucketName` → `Input.BucketName`
  - `Connection.S3Directory` → `Input.SourceDirectory` (renamed)
  - `Connection.SearchPattern` → `Input.SearchPattern`
  - `Connection.DownloadFromCurrentDirectoryOnly` → `Input.DownloadFromCurrentDirectoryOnly`
  - `Connection.DestinationDirectory` → `Input.TargetDirectory` (renamed)

- **Moved to Options tab**:
  - `Connection.FileLockedRetries` → `Options.FileLockedRetries`
  - `Connection.DeleteSourceObject` → `Options.DeleteSourceObject`
  - `Connection.ThrowErrorIfNoMatch` → `Options.ThrowErrorIfNoMatch`
  - `Connection.DestinationFileExistsAction` → `Options.ActionOnExistingFile` (renamed)

- **Updated in Connection tab**:
  - `Connection.AWSCredentials` → `Connection.AwsCredentials` (renamed)
  - `Connection.PreSignedURL` → `Connection.PreSignedUrl` (renamed)

### New Features
- **Enhanced Error Handling**: Added comprehensive error handling with new Options properties:
  - `ThrowErrorOnFailure` (bool, default: false) - Controls whether errors throw exceptions
  - `ErrorMessageOnFailure` (string, default: "") - Custom error message for failures
- **Improved Result Structure**: 
  - Renamed `Data` property to `Objects` in Result class
  - Added structured `Error` property with `Message` and `AdditionalInfo` fields
- **Error Handler Helper**: New static ErrorHandler class for consistent error processing

### Migration Guide
To upgrade to the new version:
1. **Input tab**: Select the same values for BucketName, SourceDirectory (previously S3Directory), SearchPattern, DownloadFromCurrentDirectoryOnly, and TargetDirectory (previously DestinationDirectory) as they were in the Connection tab
2. **Options tab**: Configure FileLockedRetries, DeleteSourceObject, ThrowErrorIfNoMatch, and ActionOnExistingFile (previously DestinationFileExistsAction) with the same values as they were in the Connection tab
3. **Connection tab**: Update references to AwsCredentials (previously AWSCredentials) and PreSignedUrl (previously PreSignedURL)
4. **Result handling**: Update code that references `result.Data` to use `result.Objects` instead
5. **Error handling**: Consider setting `ThrowErrorOnFailure` to true and providing custom `ErrorMessageOnFailure` if needed

### Technical Improvements
- Separated concerns by organizing parameters into logical groups (Input, Connection, Options)
- Implemented consistent error handling pattern following Frends task guidelines
- Added proper error reporting with structured Error objects
- Improved naming consistency (AWS → Aws, URL → Url)
- Enhanced maintainability and testability with better class structure


## [2.2.0] - 2024-12-11
### Updated
- Listing updated to newer version.
## [2.1.0] - 2023-05-10
### Fixed
- Changed the way the Task handles downloaded objects to fix blank PDF files.
### Changed
- Result change: List name change from 'Results' to 'Data'.
- Input parameter name changed from to 'Connection'.

- Memory leak fix.
- SingleResultObject parameter changes:
	- New parameters: Overwritten, SourceDeleted, Info.
	- Removed parameter: ObjectData (replaced by Info).

## [2.0.0] - 2022-12-02
### Modified
- Memory leak fix.
- SingleResultObject parameter changes:
	- New parameters: Overwritten, SourceDeleted, Info.
	- Removed parameter: ObjectData (replaced by Info). 

## [1.0.0] - 2022-05-09
### Added
- Initial implementation 