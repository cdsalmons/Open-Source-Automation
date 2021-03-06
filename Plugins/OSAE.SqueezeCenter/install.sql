CALL osae_sp_object_type_add ('SQUEEZEBOX','Squeezebox Device','','THING',0,0,0,1);
CALL osae_sp_object_type_state_add('SQUEEZEBOX','ON','On');
CALL osae_sp_object_type_state_add('SQUEEZEBOX','OFF','Off');
CALL osae_sp_object_type_event_add('SQUEEZEBOX','ON','On');
CALL osae_sp_object_type_event_add('SQUEEZEBOX','OFF','Off');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','PLAY','Play','Item','','','');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','STOP','Stop','','','','');
CALL osae_sp_object_type_method_add('SQUEEZEBOX','SHOW','Display Message','message','','','');

CALL osae_sp_object_type_add ('SQUEEZEBOX SERVER','Squeezebox Server','','PLUGIN',1,1,0,1);
CALL osae_sp_object_type_state_add ('SQUEEZEBOX SERVER','ON','Running');
CALL osae_sp_object_type_state_add ('SQUEEZEBOX SERVER','OFF','Stopped');
CALL osae_sp_object_type_event_add ('SQUEEZEBOX SERVER','ON','Started');
CALL osae_sp_object_type_event_add ('SQUEEZEBOX SERVER','OFF','Stopped');
CALL osae_sp_object_type_method_add ('SQUEEZEBOX SERVER','ON','Start','','','','');
CALL osae_sp_object_type_method_add ('SQUEEZEBOX SERVER','OFF','Stop','','','','');
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','Server Address','String','','',0);
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','CLI Port','String','9090','',0);
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','TTS Save Path','String','','',0);
CALL osae_sp_object_type_property_add ('SQUEEZEBOX SERVER','TTS Play Path','String','',0);
