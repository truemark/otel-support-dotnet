# Otel Support .Net

Provides support for dotnet OpenTelemetry instrumentation.

## How this works
![img.png](img/otel-support-java.png)

## Available library implementations

-   [TrueMark.OtelSupport library for netstandard2.0](TrueMark.OtelSupport/README.md)

## Library version management
- Version is managed automatically by the ci cd process
- In cases of bumping versions, it's best the version gets updated in the `develop` branch in this file [publish.yml](.github/workflows/publish.yml) as shown below:

```yaml
    with:
      dotnet-version: "8.0"
      develop-preid: "beta"
      branch-preid: "alpha"
      bumped-version-tag: "1.0.0" # bump this version manually in develop branch
```