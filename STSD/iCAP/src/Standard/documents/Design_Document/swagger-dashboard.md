DashboardAPI
============
iCAP Dashboard API

**Version:** v1


### /DashboardAPI/Get
---
##### ***GET***
**Summary:** 1. Get company dashboard

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get company dashboard success |
| 403 | The identity token not found |

### /EventAPI/GetAll
---
##### ***GET***
**Summary:** 2. Get all event logs

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get all event data success |
| 403 | The identity token not found |

### /EventAPI/GetNew
---
##### ***GET***
**Summary:** 3. Get new event logs

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get new event data success |
| 403 | The identity token not found |

### /EventAPI/GetDone
---
##### ***GET***
**Summary:** 4. Get already done event logs

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get already done event data success |
| 403 | The identity token not found |

### /EventAPI/Update
---
##### ***PUT***
**Summary:** 5. Update Event Log

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |
| eventData | body | The event data needs to updated. | No | [EventDataTemplate](#eventdatatemplate) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Update event data success |
| 403 | The identity token not found |
| 406 | Update event data fail |

### /EventAPI/SetEmail
---
##### ***PUT***
**Summary:** 6. Set the EmailSetting.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |
| payload | body | The email setting data needs to updated. | No | [EmailSettingTemplate](#emailsettingtemplate) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Success |
| 400 | Payload data error. |
| 403 | 1. The identity token not found  2. User do not have enough authorization. |
| 406 | Payload is null or update fail. |
| 500 | Update fail. |

### /EventAPI/GetEmailList
---
##### ***GET***
**Summary:** 7. Get the email setting information.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| CompanyId | query |  | No | integer |
| token | header | The administrator identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get email setting success |
| 400 | Request does not contain CompanyId |
| 403 | The identity token not found |
| 404 | The email data not found |

### /EventAPI/DeleteEmail
---
##### ***DELETE***
**Summary:** 8. Delete email

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |
| emailFrom | header | The sender email | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Delete email success |
| 304 | Delete email fail |
| 403 | The identity token not found |
| 404 | email not found |

### /EventAPI/SendEmail
---
##### ***POST***
**Summary:** 9. Send the email.
P.S. : If the email which is used to send the message has been authenticate by cellphone, Need to set the gmail low safety.
Otherwise the smtp sever will reject log in.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The administrator identity token | No | string |
| payload | body | The ema | No | [EmailSendingInfoTemplate](#emailsendinginfotemplate) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Send email success. |
| 400 | 1. Request does not contain deviceName.  2. The input email-sending information is null.  3. Send email fail.Please check the port number and the ssl and ensure they are correct. |
| 403 | The identity token not found |
| 404 | 1. The device was not found in the database.   2. The email was not found or the field enable is false. |

### /SettingAPI/GetThreshold
---
##### ***GET***
**Summary:** 8. Get threshold setting

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get threshold setting success. |
| 403 | The identity token not found |

### /SettingAPI/SetThreshold
---
##### ***PUT***
**Summary:** 9. Set threshold setting

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |
| data | body | The threshold setting data | No | [ [ThresholdSettingTemplate](#thresholdsettingtemplate) ] |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Set threshold setting success |
| 304 | Insert threshold data error |
| 400 | Threshold setting data error |
| 403 | The identity token not found |

### /WidgetAPI/Create
---
##### ***POST***
**Summary:** 10. Create the widget.

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The administrator identity token | No | string |
| widgetData | body | The widget data | No | [WidgetTemplate](#widgettemplate) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 201 | Create widget success |
| 403 | The identity token not found |
| 406 | Widget data error |
| 417 | Expection Failed |

### /WidgetAPI/Get
---
##### ***GET***
**Summary:** 11. Get the widget information

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| Id | query | The widget Id | No | integer |
| token | header | The administrator identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 200 | Get widget data success |
| 400 | Request does not contain Id |
| 403 | The identity token not found |
| 404 | The widget data not found |

### /WidgetAPI/Update
---
##### ***PUT***
**Summary:** 12. Update widget

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| token | header | The identity token | No | string |
| widgetdata | body | The widget data | No | [WidgetTemplate](#widgettemplate) |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Update widget success |
| 403 | The identity token not found |
| 404 | widget not found |
| 406 | widget data error |

### /WidgetAPI/Delete
---
##### ***DELETE***
**Summary:** 13. Delete widget

**Parameters**

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| Id | header | The widget Id | No | string |
| token | header | The identity token | No | string |

**Responses**

| Code | Description |
| ---- | ----------- |
| 202 | Delete widget success |
| 403 | The identity token not found |
| 404 | Widget not found |
| 406 | Error on delete |

### Models
---

### EventDataTemplate  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| eventId | string |  | No |
| time | string |  | No |
| eventclass | string |  | No |
| devName | string |  | No |
| info | string |  | No |
| level | integer |  | No |
| owner | string |  | No |
| isChecked | boolean |  | No |

### EmailSettingTemplate  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| deviceName | string |  | No |
| smtpAddress | string |  | No |
| portNumber | integer |  | No |
| enableSSL | boolean |  | No |
| emailFrom | string |  | No |
| password | string |  | No |
| enable | boolean |  | No |

### EmailSendingInfoTemplate  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| deviceName | string |  | No |
| class | string |  | No |
| info | string |  | No |

### ThresholdSettingTemplate  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| class | string |  | No |
| value | double |  | No |
| name | string |  | No |
| enable | integer |  | No |
| func | integer |  | No |

### WidgetTemplate  

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| id | integer |  | No |
| name | string |  | No |
| dataId | integer |  | No |
| dataCount | integer |  | No |
| chartType | string |  | No |
| width | string |  | No |
| settingStr | string |  | No |