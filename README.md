# SW20190530_Ver3
*Agiltron Switch Evaluation Toolkit*  
Basic format and UI style for Agiltron Switch programs

## TO RUN:
- Download  <a href="https://github.com/MaeZhao/SW20190530_Ver3">git project</a> and run <a href="https://github.com/MaeZhao/SW20190530_Ver3/blob/Without_Diagrams/SW20190530_Ver3/bin/Debug/SW20190530_Ver3.exe">SW20190530_Ver3.exe</a> at ```../SW20190530_Ver3/bin/Debug/SW20190530_Ver3.exe```

## TO DEBUG:
- Install Visual Studio 2017 using .Net 4.7.1

## COMPLETED MILESTONES:
- Created all application functions and interfaces from scratch based on a previous switch control application coded in C++
- Completed UI for:
	- New Switch Control Creation
	- Add Step
	- Run Step Animations
	- Pause/Go Animations
	- Stop Animations
	- Progress Bar

## COMMON ERRORS/VS 2017 GLITCHES
- ``` Partial declarations of 'WindowUIComponents' must not specify different base classes ```  
  - If all xaml and cs files are declared correctly (ex: using WindowUIComponents instead of Window), close VS and delete _bin_ and _obj_ files in project folder. Clean and rebuild.  
  - Visual Studio 2017 has trouble accessing Resource dictionaries in folders (i.e. Resources folder)  
## COMMON DOCUMENTATION SYNTAX  
- "switchGrid" is referenced also as "switch table". Both refer to the on/off switch table displayed in OpticalControlSequence window.

## EXTERNAL LIBRARIES USED/USEFUL DOCUMENTATION  
- _ToastNotifications v2_ is an external library used to generate the live pop up notifications see: https://github.com/rafallopatka/ToastNotifications for documentation and usage.
  - General format for generating notifications used in this app is:
    - ``` Notification notifier.ShowInformation(String message, MessageOption messageOption); ```
	  - ``` Notification notifier ``` is the Notification object
	    - In OpticalSwitchControlSequence the custom Notification object is the field _notifier_
	  - ``` ShowInformation ``` is one of the 4 different default styles options for the messages
	    - The other defaults are:
		  ``` 
		  notifier.ShowSuccess(...);
		  notifier.ShowWarning(...);
		  notifier.ShowError(...);  
	  - ``` String message ``` contains a string with the custom message, example: "Test Paused"
	  - ``` MessageOption messageOption ``` is the custom settings set for the notifications of the window instance
	    - In OpticalSwitchControlSequence the custom setting is the field _messageOption_
