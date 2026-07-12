# Bootstrap

Brief info about boot strap, instances, static ctor, validating
Future idea:

## If startup discovery grows, split into:

### FrameworkDirectoryScanner

* Receives known directories.
* Scans directories in parallel using `Methods.Parallel()`.
* Produces discovered files with metadata.

Example sources:

* Root directory
* Configs directory
* Configurations directory
* Parent directories (up to configured limit)

### FrameworkFileLoader

* Loads discovered files in parallel.
* Resolves loading based on file type:

  * json/xml/config → configuration loaders
  * pub/priv/enc → encryption loaders
  * certificates → certificate loaders

Loaded results are stored in a shared `ScannedData` collection.

### ScannedData

A thread-safe list of snapshot objects, containing discovered and loaded objects.

Examples:

* `IConfigurationRoot`
* `KeyData`
* Certificates
* Other framework startup resources

The purpose is that bootstrap modules do not perform file discovery or file loading themselves.

### FrameworkBootLoader

Runs independent bootstrap work while file scanning/loading continues:

* Initializes environment, CLI arguments and environment variables.
* Creates initial configuration builder.
* Performs async boundary warmup.
* Initializes Base62 and converters.
* Creates default ServiceProvider with DataProtection.

After discovery and loading complete, normal bootstrap continues.

### Boot modules

Boot modules become lookup/assignment steps only.

Example:

`ConfigBoot`

* Finds `IConfigurationRoot` from `ScannedData`.
* Assigns `ConfigInstance`.

`CryptographyBoot`

* Finds encryption keys from `ScannedData`.
* Assigns `CryptographyInstance`.

The final flow becomes:

Filesystem
→ DirectoryScanner
→ FileLoader
→ ScannedData
→ Boot modules
→ Instance classes

Benefits:

* Filesystem discovery happens once.
* No duplicate searches.
* No duplicate file loading.
* Bootstrap modules become deterministic.
* Runtime code only consumes initialized framework state.
