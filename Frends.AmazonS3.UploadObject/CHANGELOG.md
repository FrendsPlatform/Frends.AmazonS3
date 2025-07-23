# Changelog

## [3.0.0] - 2025-07-23
### Changed
- Updated NuGet version to 3.0.0

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
