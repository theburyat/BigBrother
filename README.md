# Big Brother Server App

## How to run

0. Prerequisites:
   * sqlite
   * docker-compose


1. Clone the repository:
```
git clone https://github.com/theburyat/BigBrother.git
```
2. Go into the repository folder:
```
cd ./BigBrother 
```
3. Build images:
```
docker compose build
```
4. Run containers:
```
docker compose up
```

That's all! Open `http://localhost:444/` in your browser for getting to the administration page