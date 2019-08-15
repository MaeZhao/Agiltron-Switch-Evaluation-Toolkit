# SW20190530_Ver3
*Agiltron Switch Evaluation Toolkit*  
Basic format and UI style for Agiltron Switch programs

## COMMON ERRORS/VS 2017 GLITCHES
- ``` Partial declarations of 'WindowUIComponents' must not specify different base classes ```  
  - If all xaml and cs files are declared correctly (ex: using WindowUIComponents instead of Window), close VS and delete _bin_ and _obj_ files in project folder. Clean and rebuild.
