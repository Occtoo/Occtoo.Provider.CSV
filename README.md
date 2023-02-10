# Example of a CSV data provider to Occtoo
To run the solution you will need to add your data provider ID and secret as [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows#set-a-secret) in the project.

Example:
```sh
dotnet user-secrets set "providerid" "12345"
```
Check our documentation on [how to set up a Provider in Occtoo Studio](https://docs.occtoo.com/docs/get-started/provide-data#12-create-data-provider)


The program reads the file Product_Data_Sample.csv and converts each row into a Dynamic Entity. They are then imported into Occtoo using the [OnboardingServiceClient](https://github.com/Occtoo/Occtoo.Onboarding.Sdk)

