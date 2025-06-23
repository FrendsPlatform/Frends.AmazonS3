# Changelog

## [2.0.0] - 2025-06-23
### Changed
- Made Task Harmonization changes
	- Moved Connection.BucketName to Input
	- Renamed ACL to Acl inside Connection
	- Moved Connection.ObjectLockEnabledForBucket to Options and renamed to ObjectLockEnabled
	- Refactored CreateBucket.cs to function with Harmonization changes/additions
	- Made change to target .NET 8 in main and unit test projects instead of .NET 6
### Added
- Made Task Harmonization additions
	- Created Input definition file
	- Created Options definition file
	- Added ThrowErrorOnFailure to Options 
	- Added ErrorMessageOnFailure to Options
	- Added BucketName to Result
	- Added Error to Result
	- Added migrations file

## [1.0.0] - 2023-07-28
### Added
- Initial implementation