# Changelog

## [2.1.0] - 2026-02-23
### Fixed
- Ensure FrendsTaskMetadata.json is included in NuGet package

## [2.0.0] - 2025-06-24
### Changed
- [Breaking] Created Input tab and moved Connection.BucketName to Input
  - To upgrade to the new version, add the same value in the same field now on the Input tab as it was on the Connection tab
- ACL renamed to Acl in Connection
- [Breaking] Connection.ObjectLockEnabledForBucket moved to Options and renamed ObjectLockEnabled
  - To upgrade to the new version, add the same value in the renamed field now on the Options tab as it was on the Connection tab.
- Refactored CreateBucket.cs to align with harmonized input/options model
- Updated target framework from .NET 6 to .NET 8

### Added
- Created Options definition file
- Added ThrowErrorOnFailure to Options
- Added ErrorMessageOnFailure to Options
- Added BucketName to Result
- Added Error to Result
- Added migrations file

## [1.0.0] - 2023-07-28
### Added
- Initial implementation