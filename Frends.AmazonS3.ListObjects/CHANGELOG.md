# Changelog

## [2.1.0] - 2025-12-29		
### Changed		  
- Updated AWS SDK version.		

## [2.0.0] - 2025-07-17

## [Breaking] Major refactoring with new class structure and error handling

### Added
- **Connection class**: New dedicated class for AWS connection parameters
  - `AwsAccessKeyId` - AWS access key ID for authentication
  - `AwsSecretAccessKey` - AWS secret access key for authentication  
  - `Region` - AWS region for the S3 service
- **Options class**: New class for task configuration options
  - `ThrowErrorOnFailure` - Boolean flag to control error throwing behavior (default: false)
  - `ErrorMessageOnFailure` - Custom error message for failures (default: empty string)
- **Error class**: New structured error handling
  - `Message` - Error message string
  - `AdditionalInfo` - Dynamic property for additional error context
- **ErrorHandler class**: New static utility class for centralized error handling
  - `Handle` method for consistent error processing across the task
- **Result.Success**: New boolean property indicating task execution success
- **Result.Error**: New error property for structured error information

### Changed
- **[Breaking]** Renamed `Source` class to `Input` class
- **[Breaking]** Moved AWS credentials from Input to Connection class:
  - `Input.AwsAccessKeyId` → `Connection.AwsAccessKeyId`
  - `Input.AwsSecretAccessKey` → `Connection.AwsSecretAccessKey`
  - `Input.Region` → `Connection.Region`
- **[Breaking]** Renamed `Result.ObjectList` to `Result.Objects`
- **[Breaking]** Task method signature now includes Connection and Options parameters
- Improved error handling with consistent error structure and configurable error behavior

## [1.0.0] - 2022-04-25
### Added
- Initial implementation