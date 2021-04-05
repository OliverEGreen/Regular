#Examples

Let's walk through a basic example: creating a rule to ensure consistent door numbers. In Revit, this often means targeting the 'Mark' parameter for all elements of the 'Door' category. Our theoretical company states that all door codes must either begin with **'ID'** for internal doors, or **'ED'** for external doors. This is followed by a **full-stop**, and then **3 numbers**, for instance `'ID.001'` or `'ED.101'`.

- In our case, we're only accepting values which are a full match for our door numbering standard, so Match Type is set to 'Full'
- We can start with defining an Option Set, to allow for both 'ID' and 'ED'.
- We then add the Full Stop rule part.
- Following this, we add in the 'Any Number' rule part 3 times. 

That's it! The rule now meets our company standards. We can execute the rule from the Rule Manager to validate all our doors numbers
