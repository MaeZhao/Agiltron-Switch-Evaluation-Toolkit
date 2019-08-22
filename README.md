# SW20190530_Ver3
*Agiltron Switch Evaluation Toolkit*  
Basic format and UI style for Agiltron Switch programs

## COMMON ERRORS/VS 2017 GLITCHES
- ``` Partial declarations of 'WindowUIComponents' must not specify different base classes ```  
  - If all xaml and cs files are declared correctly (ex: using WindowUIComponents instead of Window), close VS and delete _bin_ and _obj_ files in project folder. Clean and rebuild.

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
