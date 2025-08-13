# Changelog

## [3.0.0] - 2025-07-23

- [Breaking] Renamed `FilePath` to `SourceDirectory` in Input class for better clarity
- [Breaking] Renamed `S3Directory` to `TargetDirectory` in Input class for consistency
- [Breaking] Moved `BucketName` from Connection to Input class as it's essential for task function
- [Breaking] Moved `UploadFromCurrentDirectoryOnly` from Connection to Input class
- [Breaking] Moved `PreserveFolderStructure` from Connection to Input class
- [Breaking] Moved `DeleteSource` from Connection to Input class
- [Breaking] Renamed `AWSCredentials` to `AwsCredentials` in Connection class following PascalCase conventions
- [Breaking] Renamed `PreSignedURL` to `PreSignedUrl` in Connection class following PascalCase conventions
- [Breaking] Moved `PartSize` from Input to Connection class as it's connection-related
- [Breaking] Moved and renamed `UseACL` to `UseAcl` in Connection class following PascalCase conventions
- [Breaking] Moved and renamed `ACL` to `Acl` in Connection class following PascalCase conventions
- [Breaking] Created new Options class with standard `ThrowErrorOnFailure` and `ErrorMessageOnFailure` properties
- [Breaking] Moved `ThrowErrorIfNoMatch` from Connection to Options class
- [Breaking] Renamed `UploadedObjects` to `Objects` in Result class for consistency
- [Breaking] Task method signature now follows standard pattern: Input, Connection, Options parameters

## [2.0.0] - 2025-05-21
### Changed
- Updated AWS SDK to version 4.0.0.5
- Changed the logging logic to use custom logger
### Added
- [Breaking]Added GatherDebugLog option to allow disabling the DebugLog, `false` by default
  - By default, the gathering debug log is now disabled.
  - To replicate the previous task behaviour set GatherDebugLog parameter to `true`

## [1.3.0] - 2025-03-13
### Fixed
- Fixed issue where ThrowErrorIfNoMatch was not correctly handled. Now, no error is thrown if set to false, and an error is thrown if set to true when no files match."

## [1.2.0] - 2023-08-02
### Added
- Added multipart upload feature, which allows transferring files larger than 5GB.

## [1.1.1] - 2023-06-13
### Changed
- Connection.ThrowExceptionOnErrorResponse documentation update.
- Improved exception handling.

## [1.1.0] - 2023-02-07
### Changed
- Task returns an object instead of list.
### Added
- Result.Success to indicate that operation was completed without errors.
- Result.DebugLog to contain log of each operation.
- Connection.ThrowExceptionOnErrorResponse to choose whether an error throws an exception or is logged to Result.DebugLog.

## [1.0.0] - 2022-05-05
### Added
- Initial implementation
