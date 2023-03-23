// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  //BaseURL: "https://chatportalservice.azurewebsites.net/",
  //BaseURL:"http://localhost:19081/ChatServiceFabric/Gateway/",
  //signalRBaseURL:"http://localhost:19081/ChatServiceFabric/NotificationService/",
  BaseURL:"https://chatgatewaymanagaement.azure-api.net/",
  signalRBaseURL:"http://chatserivefabric.francecentral.cloudapp.azure.com:19081/ChatServiceFabric/NotificationService/",
  GoogleClientId: '468618361803-deviechkcmr3asjciqm0kfr00rovcn0e.apps.googleusercontent.com',
  IsProd:true
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
