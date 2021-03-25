# Regular
 
Regular is an open-source plugin for Autodesk Revit, intended to make facilitate an array of data validation tasks. 
DataSpec, our first too, makes data format validation easier, by letting users define rules for their data formats in simple terms. 
The plugin is able to keep track of datasets within a Revit model and live-validate them according to custom-defined rules.
If a data requirement changes (for instance a sheet numbering schema need to change) Regular's rule editor will help you update and revalidate all rules within seconds.

Behind the scenes, Regular is an MVVM WPF-based plugin, using Revit's ExtensibleStorage API.
Users define data format rules piece by piece, which are translated to Regular Expressions, and against which their data can be validated. 

Regular was started at the Thornton Thomasetti AEC Tech Hackathon 2019 in New York, presented by team 'Regular Espressos'.
