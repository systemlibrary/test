namespace SystemLibrary.Common.Framework.App;

// NOTE: OutputCache is sealed by Microsoft.
// This prevents extending it for custom cache-key generation.
// Defining OutputCache behavior with flexible keys is therefore too time consuming at the time.
// Attribute-based support is postponed (and may never be implemented).