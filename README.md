# Regular
 
Regular is an open-source plugin for Autodesk Revit, written by Oliver Green.  
 
Regular is intended to make data validation easier, by letting users define rules for their data formats in simple terms.  
The plugin is able to keep track of user-specified datasets within a Revit model and live-validate them according to custom-defined rules.
If a data requirement changes (for instance a sheet numbering schema need to change) Regular's rule editor will help you update and revalidate all rules within seconds.

Behind the scenes, Regular is an MVVM WPF-based plugin, using Revit's Dynamic Model Update and ExtensibleStorage APIs. Users define data format rules piece by piece, which are translated to Regular Expressions, and against which their data is validated. 

As of 3rd March 2021, the code remains sliiightly buggy. But we're getting there. Features to add in the near future include:

- Printing Reports
- Data Palette
- JSON Rule Export/Import

Regular was started at the Thornton Thomasetti AEC Tech Hackathon 2019 in New York, presented by team 'Regular Espressos'.
