docker build --build-arg NEXUS_USERNAME --build-arg NEXUS_PASSWORD --file ./buildTestPublish.Dockerfile --target application -t application .
docker run --env "ASPNETNETCORE_ENVIRONMENT=Development" -p 127.0.0.1:5000:5000/tcp application
