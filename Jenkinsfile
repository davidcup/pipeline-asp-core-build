
def version = "1.0.0"
def slackChannel = "alerts"


pipeline {

    options { 
          disableConcurrentBuilds() 
	      buildDiscarder(logRotator(numToKeepStr: '5', artifactNumToKeepStr: '5'))
	}

    agent {
		node {
		  label 'dotnetcore'
		  customWorkspace '/home/david/jenkins/workspace/github-davidcup'
		}
    }	
  
    stages {
	    stage('Restore') {		
             steps {		
		     	sh(script: 'dotnet restore AspNetCoreApiDemo.sln', returnStdout: true)	
			 }
	    }
		  
		stage("Sonar Begin") {
            withCredentials([string(credentialsId: 'sonar-token', variable: 'sonar-token')]) {
            sh 'dotnet sonarscanner begin 
			   /k:"aspnetcore-apidemo" 
			   /v:${version}
			   /d:sonar.host.url="http://10.0.0.11:9095/" 
			   /d:sonar.login="$sonar-token" 
			   /d:sonar.exclusions="**/obj/**, **/bin/**" 
			   /d:sonar.cs.opencover.reportsPaths=".sonarqube/coverage/api.opencover.xml"' 
            }
        }  
		  
        stage('Build') {
            steps {
               sh "dotnet build AspNetCoreApiDemo.sln -c Release"
            }
        }
		
		stage("Sonar End") {
			withCredentials([string(credentialsId: 'sonar-token', variable: 'sonar-token')]) {
				sh 'dotnet sonarscanner end /d:sonar.login="$sonar-token"'
			}
		}
    }
  
}