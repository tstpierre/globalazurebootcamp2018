# LAB03 - Roomviewer App

## Wire to your API endpoint
1. Open both `./src/environments/environment.ts` and  `./src/environments/environment.prod.ts`
1. Set property `roomBaseUrl` value to your API endpoint from Azure
1. Test changes with `ng serve` and opening browser to <a href="http://localhost:4200" target="_blank">http://localhost:4200</a>
1. Commit and push to your GitHub fork/repo

## Create Web App Resource
1. Log in to Azure Portal
1. Click "Create a resource"
1. Type `web app` in the search box and press Enter
1. Select `Web App` and click `Create` button
1. Enter `rv{attendeeNumber}` for the App name
1. Select `Use existing` resource group and select `GlobalAzureBootcamp2018` from the dropdown
1. Select the App Service plan/Location to open the configuration blade
1. Select `Create new`
1. Enter `GlobalAzureBootcamp2018ServicePlan` for the App Service plan name
1. Select `West US 2` from the Location dropdown
1. Select the Pricing tier to open the configuration blade
1. Select `F1 Free` and click `Select` button
1. Select `OK` button on the New App Service Plan blade
1. Select `Windows` for OS
1. Select `Off` for Application Insights
1. Select `Create`
1. You should see a `Deployment in progress` notification followed shortly with a `Deployment succeeded`
1. Test the new web app

## VSTS Build
1. Select `Builds` from the Build and Release toolbar menu
1. Click `+ New` or `+ New definition` button
1. Select `GitHub` as a source
1. Select `Authorize using OAuth` (may have to allow popups)
1. Login / Authorize the OAuth connection (may have to confirm password with GutHub)
1. Select the `...` next to the Repository field
1. Filter and select your fork/repo and click `Select`
1. Select `Continue`
1. Select `Empty Process` at the top of the template page
1. Select `+` button on the Phase 1 line
1. Type `npm` to filter tasks for npm, add the `npm` task
1. Select the added `npm` task
1. Enter `labs/03 - web ci-cd` for Working folder with package.json
1. Add another `npm` task after the `npm install` task
1. Select the added `npm` task
1. Select `Custom` from the Command dropdown
1. Enter `labs/03 - web ci-cd` for the Working folder with package.json
1. Enter `run build` for the Command and arguments
1. Add a new task, this time `Archive Files`
1. Select added `Archive` task
1. Enter `labs/03 - web ci-cd/dist` for Root folder or file to archive
1. Uncheck Prepend root folder name to archive paths
1. Add a new task, this time `Publish Build Artifacts`
1. Select added 'Publish` task
1. Enter `$(Build.ArtifactStagingDirectory)` for Path to publish
1. Enter `drop` for Artifact name
1. Select `Save & Queue`
1. Look at the build summary and artifacts (the zip file is attached)

### If you own the repo (GitHub security)
Caveat, this whole process works best using VSTS GIT repos, as there is no OAuth and external permission hiccups to wire it all up seamlessly.

1. Edit the build definition
1. Select `Triggers` tab
1. Check `Enable continuous integration` and `Batch changes while a build is in progress`
1. Save the build definition

## VSTS Release
1. From the Summary tab of the successful build, select `Create release` in the Deployments section
1. Select `Apply` on the `Azure App Service Deployment` template
1. Select `1 phase, 1 task` in the `Environment 1` box
1. Select your azure subscriptionfrom the Azure subscription dropdown
1. Click `Authorize` to connect to the subscription
1. Select your web app previously created in the App service name dropdown
1. Select `Deploy Azure App Service` task already added under the `Run on agent`
1. Expand `Additional Deployment Options`
1. Check `Publish using Web Deploy`
1. Check `Remove additional files at destination`
1. Select `Save`
1. Select `Create release` from the `+ Release` menu
1. Once deployment is finished, visit your web app URL and claim victory!

## Putting it all together
1. The release trigger is made for you when you create it off of the successful build page like we did
1. When you push changes to GitHub fork/repo a build will kick off (if you owned the repo and had rights to enable trigger)
1. When the build finishes (successfully) a new release will be created for that build
1. That release will automatically deploy to your Azure Web App resource
1. This is a starting point for things like dev, qa and stage environments where builds can be tested and approved before moving to the next environment, without requiring a new build process