# RaycastHit

A sample demonstrates hit detection with ray cast. It show how to trigger ray cast hit detection about three different messages, enter, stay and exit messages.

Hold down a left mouse button and move mouse cursor around the cylinder. Ray cast hit messages are sent to Cylinder and it put the subsribed message into Console like the following:

```
Cylinder: onRaycastEnter : first time cursor onto the cylinder
Cylinder: onRaycastStay : the cursor stays onto the cylinder
Cylinder: onRaycastStay
Cylinder: onRaycastExit : the cursor is about to exit from cylinder
```
