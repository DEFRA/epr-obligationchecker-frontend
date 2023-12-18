run: ## Run the application
	./buildscripts/run.sh

all-tests: ## Run unit tests
	./buildscripts/all-tests.sh

integration-tests: ## Run unit tests
	./buildscripts/integration-tests.sh

unit-tests: ## Run unit tests
	./buildscripts/unit-tests.sh

redis-up: ## Run Redis docker instance
	docker run -p 6379:6379 -d --name redis-session redis

redis-down: ## Stop Redis docker instance
	docker stop redis-session

-include buildscripts/util.make


