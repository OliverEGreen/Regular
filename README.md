# Regular

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

## TL;DR
Regular is an open-source Revit plugin, designed to help users manage construction data. The first tool, DataSpec, lets users define flexible format rules for Revit parameter values. Data can then be validated (and, if necessary, corrected) using an intuitive user interface. 


## The Vision

It is frequently stated that construction technology suffers from a lack of common, established data standards. It is this standards vacuum which underlies several problems faced within the industry every day:

1. Agreeing on optimal data formats for is hard: opinions are strong and will often differ, between companies and sectors that have already invested in establishing their own internal standards.  
2. Validating data created in line with any format specifications is a specialist task that too often falls to those originally trained as architects, engineers, or project managers - not data managers.
3. The few standards we have are prone to frequent change. Moving the goalposts mid-way through a project often results in costly and unappealing manual data correction work. The risk of this happening is great enough to deter further standardisation from taking place.

The second and third problem above can be overcome with programming skills, which are in short supply.

Enter DataSpec, designed to address the 3 problems above.

- DataSpec gives users control over establishing data format standards at whatever level is right for them, be it project-specific, company-wide or industry-wide standards.
- It provides an intuitive interface for rapid, automatic format validation against these rules. 
- It lets users easily adjust rules, and rapidly revalidate against new standards, should these be introduced. 

>Regular is an ongoing project. Future tools are currently being developed to help further reduce the cost of AEC data management.


## How It Works
Behind the scenes, DataSpec uses **regular expressions** - a technology commonly used by programmers to define rules for validating data that needs to be follow a specific format, such as email addresses or phone numbers. Regular expressions (often shortened to 'regex') are an incredibly powerful tool, but one which requires specialist programming knowledge. Even programmers can find them difficult to work with!

DataSpec works by abstracting away the complexities of regex behind an intuitive user interface, which lets non-technical users define rules for their data formats in plain English. Our goal is to >'bring Regular Expressions to Regular Users'. 

Data format rules are stored locally in a Revit document using the ExtensibleStorage API. These can be exported and imported between files using the .json file format, allowing for data format standards to be defined for a project, a company or even shared industry-wide. Those wishing to define a universal data format for, say, park benches are encouraged to pick up the mantle.


## Wiki Guidance
- [Installation](https://github.com/OliverEGreen/Regular/wiki/DataSpec-Installation)
- [Getting Started](https://github.com/OliverEGreen/Regular/wiki/DataSpec-Getting-Started)
- [Examples](https://github.com/OliverEGreen/Regular/wiki/DataSpec-Examples) for step-by-step examples and links to video guides


## License & Data Safety

- All data is processed locally on your machine, no external connections are made to anywhere or anything.
- Regular is available using the highly-permissive GNU Public License v3.0. See our [license file](https://github.com/OliverEGreen/Regular/edit/master/LICENSE) for full details.

Key Dependencies:
- [Newtonsoft JSON.NET](https://www.newtonsoft.com/json)
- [CSVHelper](https://joshclose.github.io/CsvHelper/)
- Regex validation relies on the .NET [System.Text.RegularExpressions](https://docs.microsoft.com/en-us/dotnet/api/system.text.regularexpressions?view=net-5.0) library.


## Current Limitations
- DataSpec can only validate text-based parameter values.
- Conditional logic cannot be applied within rules - only relatively simple format logic can be used. 
- For simplicity's sake, DataSpec uses an intentionally-limited subset of the full regular expression syntax. AEC data formats tend to be relatively simple, so it's better to cover 99% of cases than push for 100%.


## Contributing
If you'd like to help contribute to Regular, feel free to raise any issues, create pull requests and/or [message me on Twitter](https://twitter.com/Oliver_E_Green).  
See our [CONTRIBUTING.md](https://github.com/OliverEGreen/Regular/blob/master/CONTRIBUTING.md) file for more details. 


## Genesis
Regular was started at the Thornton Tomasetti AEC Tech Hackathon 2019 in New York, presented by team ☕ **Regular Espressos** ☕ 
