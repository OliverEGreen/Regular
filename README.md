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


## Installation
For installation notes, see [INSTALLATION.md](https://github.com/OliverEGreen/Regular/blob/master/INSTALLATION.md)


## Using DataSpec
Enough soapboxing, let's learn how to use DataSpec!

### Creating A Rule

1. To get started, click on the DataSpec Rule Manager button in the Regular ribbon tab. This will launch the Rule Manager window.
2. To create a new rule, click on the '+' New Rule button.
3. Name your rule (something self-describing like 'door codes' will work best).
4. Each rule is designed to target a specific parameter for one or more Revit categories, such as walls or floors. To target categories, press the 'Show Categories' button. This will expand a side panel allowing users to multi-select as many categories as they want to target.
5. The next step is to specify the parameter value whose values are being validated against your new rule. The Target Parameter drop-down will only display parameters which all selected categories have in common. If there are no common parameters, it will tell you so.
6. The Match Type dropdown lets you specify whether your rule is intended to validate just the start or end of a parameter value, or whether it must be applied to the entire value (this is the default approach). 
7. Data format rules are built up piecemeal, as a sequence of rule parts. You can begin building a rule by selecting a rule part type from the dropdown and pressing the + button. Rule parts will appear in the lower half of the dialog, and can be added, reordered and deleted using the buttons provided.
8. To help you define the right data standards, DataSpec will auto-generate random values that are compliant with the rule you're specifying. These will update as the rule changes. Click the refresh button to generate new compliant examples as a logic check.

### Validating Data

- Once a rule has been defined, users can select the rule by clicking on it and pressing the 'Execute' button. 
- Executing a rule will expand a resizable side panel and begin creating a validation report. The UI will be frozen as this report is created.
- Once the report has run, DataSpec will report what percentage of data was valid against your chosen rule.
- Data can be sorted by clicking any of the column headers.
- Where data has been found to be invalid against the rule, a randomised value has been generated in compliance with the rule's format standards.
- For ease of use, any of the target parameter values can be edited and revalidated directly in the UI, by clicking and re-typing a new value in the cell.
- Validation reports can be easily exported to .CSV format for review in other software.

### Managing Rules
- Once defined, rules are saved into the Revit model, and can be easily accessed and edited at any time. To edit a rule, open the Rule Manager dialog and click the blue pencil icon next to a rule.
- To delete a rule, simply click the red 'x' button next to that rule.
- To duplicate a rule, as a convenient basis for creating a new rule, select a rule and click on the 'Duplicate Rule' button. The editor window will then appear. 
- Rules can be exported to or imported from .JSON files using the 'Transfer Rules' button.

### Rule Parts
DataSpec rules are defined using a series of 'rule parts', which are arranged in a specific sequence by the user. Let's examine each of these rule parts in detail:

- Any Alphanumeric (A-Z, 0-9) will allow any letter or number in a code. Case specificity can be toggled between only uppercase, lowercase or any case by clicking on the A-Z button in the rule part body. This equates to the regex `'[A-Z0-9]'`, `'[a-z0-9]'` or `'[A-Za-z0-9]'`, depending on the chosen case options.  
- Any Letter (A-Z) refers to any single letter in a code. Case specificity can be toggled between only uppercase, lowercase or any case by clicking on the A-Z button in the rule part body. This equates to the regex `'[A-Z]'`, `'[a-z]'` or `'[A-Za-z]'`, depending on the chosen case options. 
- Any Digit (0-9) allows for any single digit in a value. This equates to the regex `'\d'`
- Custom Text allows the user to specify any particular word or symbol they want. This is defined by clicking the 'Edit' button and typing in the text box once the rule part has been added. This can be useful for company names, such as 'ACME'. Protected regex symbols are automatically sanitised from any user input. This can be made case sensitive by ticking the 'Case Sensitive' checkbox.
- Option Set lets the user define several options for part of a code, any of which will be accepted by the validator. As with Custom Text, any protected regex symbols are sanitised as they are entered. These options can be made case sensitive by ticking the 'Case Sensitive' checkbox.
- Full Stop (.) is a simple shortcut for defining a '.' character.
- Hyphen (-) is a simple shortcut for defining a '-' character.
- Underscore (_) is a simple shortcut for defining a '_' character.
- Open '(' and Close ')' parentheses are simple shortcuts for defining these characters.

*Please Note: Any rule part can be made optional by ticking the 'Optional' checkbox in the rule part body.*


## Examples
For step-by-step examples and links to video guides, see [EXAMPLES.md](https://github.com/OliverEGreen/Regular/blob/master/EXAMPLES.md).


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
Regular was started at the Thornton Tomasetti AEC Tech Hackathon 2019 in New York, presented by team ☕**Regular Espressos**☕ 
