Fay Logging NLog4
===========

What is it?
-----------

It is a [Fay Logging][FayLog] facade for the [NLog 4 logging framework][NLog]. This facade povides a simple delegate logging API making logging easier, while helping to make sure any of the code required to generate a log message is not executed unless the logging level is within scope.

Quick Start
---
Below is a simplified example for logging. A more robust implementation may use a service locator or dependency inject framework to initialize the logger.

```cs
    // Initialize logger someplace
    IDelegateLogger<string> logger = new NLogSimpleLogger(LogManager.GetCurrentClassLogger());

    // Use logger as needed
    logger.Critical(() => string.Format("Some text blah {0} blah", "blah"));

    logger.Verbose(() =>
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Foo");
        builder.Append(" ");
        builder.Append("Bar");
        return builder.ToString();
    });

    // When application is done needing the logger its recommend to dispose it
    logger.Dispose();
```

[FayLog]:  https://github.com/FayLibs/Fay.Logging
[NLog]: https://github.com/NLog/NLog