![ssw-banner](./_docs/images/ssw-banner.png)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![codecov](https://codecov.io/gh/SSWConsulting/SSW.SophieBot/branch/main/graph/badge.svg?token=MYW6YPW4P3)](https://codecov.io/gh/SSWConsulting/SSW.SophieBot)
[![Build and Deploy-Prod](https://github.com/SSWConsulting/SSW.SophieBot/actions/workflows/SSW.SophieBot.prod.yml/badge.svg?branch=main)](https://github.com/SSWConsulting/SSW.SophieBot/actions/workflows/SSW.SophieBot.prod.yml)

# SSW.SophieBot

The most advanced AI for work and home.

- [Website](https://sswsophie.com/sophiebot/)
- [Videos](https://www.youtube.com/playlist?list=PLnkcTBhDzoQ0uSLzyaS_9b8b99YB7Tz3p)

# Introduction

SSW.SophieBot is an Enterprise People Bot build with Bot Framework Composer.

# Getting Started

1. [Run bot with Bot Framework Composer](_docs/Instructions-Compile.md)

# Development

1. [Definition of Ready](_docs/Definition-of-Ready.md)
2. [Preflight Checks](_docs/Preflight-Checks.md)
2. [Branching & Merging Strategies](_docs/Branching-and-Merging.md)
3. [Pre-release requirements](_docs/Sanity-Testing.md)
4. [Deploy the bot](_docs/Instructions-Deployment.md)
5. [Definition of Done](_docs/Definition-of-Done.md)

# Solution Structure

The project consists of 3 solutions as following:
- [SSW.SophieBot.EmployeeFinder.sln](bots/employee-finder/README.MD)
- [SSW.SophieBot.sln](libraries/README.MD)
- [SSW.SophieBot.Services.sln](services/README.MD)

This [utility script](_scripts/build-all.ps1) can be used to build all solutions in one click.

# Telemetry

- You can monitor the application health [here](https://github.com/SSWConsulting/SSW.SophieBot.Private/blob/main/Instruction-Azure-Resources.md#telemetry).

# Contribute

Contributions to SSW.SophieBot are welcome. Here is how you can contribute to SSW.SophieBot:

- [Submit bugs](https://github.com/SSWConsulting/SSW.SophieBot/issues) and help us verify fixes.
- [Submit pull requests](https://github.com/SSWConsulting/SSW.SophieBot/pulls) for bug fixes and features and discuss existing proposals

# License

[MIT License](LICENSE)
