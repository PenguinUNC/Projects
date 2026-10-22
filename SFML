#include <iostream>
#include <SFML/Graphics.hpp>
#include <vector>
#include <cmath>
#include <cstdlib>
#include <ctime>
#include <stack>
using namespace std;
const int TILE_SIZE = 40;
const int WALL_THICKNESS = 4;
const int MAZE_WIDTH = 21;
const int MAZE_HEIGHT = 15;
class Entity
{
protected:
    sf::Sprite sprite;
    sf::Texture texture;
    float speed = 2.4f;
public:
    Entity() {}
    bool loadTexture(const string& filename, sf::Vector2f scale = { 1,1 })
    {
        if (!texture.loadFromFile(filename))
        {
            cerr << "Не удалось загрузить " << filename << endl;
            return false;
        }
        sprite.setTexture(texture);
        sprite.setScale(scale);
        sf::FloatRect bounds = sprite.getLocalBounds();
        sprite.setOrigin(bounds.width / 2, bounds.height / 2);
        return true;
    }
    void setPosition(float x, float y) { sprite.setPosition(x, y); }
    sf::Vector2f getPosition() const { return sprite.getPosition(); }
    sf::FloatRect getBounds() const { return sprite.getGlobalBounds(); }

    void draw(sf::RenderWindow& window) const { window.draw(sprite); }
};
class Player : public Entity
{
public:
    Player(float x, float y)
    {
        if (!loadTexture("pacman.png", { 2,2 })) {
            sf::Image img;
            img.create(16, 16, sf::Color::Yellow);
            texture.loadFromImage(img);
            sprite.setTexture(texture);
            sprite.setOrigin(8, 8);
        }
        setPosition(x, y);
    }
    bool move(float dx, float dy, const vector<sf::FloatRect>& walls)
    {
        if (dx == 0 && dy == 0) return true;

        float len = sqrt(dx * dx + dy * dy);
        dx /= len;
        dy /= len;
        sf::Vector2f newPos = getPosition() + sf::Vector2f(dx * speed, dy * speed);
        sf::FloatRect newBounds(
            newPos - sf::Vector2f(sprite.getGlobalBounds().width / 2, sprite.getGlobalBounds().height / 2),
            sprite.getGlobalBounds().getSize()
        );

        for (const auto& wall : walls)
        {
            if (newBounds.intersects(wall)) return false;
        }

        setPosition(newPos.x, newPos.y);
        return true;
    }
};
class Wall
{
public:
    sf::FloatRect rect;
    Wall(float x, float y, float w, float h) : rect(x, y, w, h) {}
};
class Exit : public Entity
{
public:
    bool active = true;
    Exit(float x, float y)
    {
        if (!loadTexture("door.png", { 2,2 }))
        {
            sf::Image img;
            img.create(24, 32, sf::Color(180, 120, 60));
            texture.loadFromImage(img);
            sprite.setTexture(texture);
            sprite.setOrigin(12, 16);
        }
        setPosition(x, y);
    }
    bool checkWin(const sf::FloatRect& playerBounds) {
        if (active && playerBounds.intersects(getBounds())) {
            active = false;
            return true;
        }
        return false;
    }
};
class Skeleton : public Entity
{
private:
    sf::Clock moveClock;
    sf::Vector2f velocity;
    float changeDirInterval = 2.0f;
public:
    Skeleton(float x, float y)
    {
        if (!loadTexture("skeleton.png", { 2,2 }))
        {
            sf::Image img;
            img.create(16, 24, sf::Color(220, 220, 220));
            texture.loadFromImage(img);
            sprite.setTexture(texture);
            sprite.setOrigin(8, 12);
        }
        setPosition(x, y);
        velocity = { 1,0 };
        moveClock.restart();
    }
    void update(const vector<sf::FloatRect>& walls)
    {
        if (moveClock.getElapsedTime().asSeconds() > changeDirInterval)
        {
            int dir = rand() % 4;
            switch (dir) {
            case 0: velocity = { 1,0 }; break;
            case 1: velocity = { -1,0 }; break;
            case 2: velocity = { 0,1 }; break;
            case 3: velocity = { 0,-1 }; break;
            }
            moveClock.restart();
        }
        sf::Vector2f newPos = getPosition() + velocity * speed;
        sf::FloatRect newBounds(
            newPos - sf::Vector2f(sprite.getGlobalBounds().width / 2, sprite.getGlobalBounds().height / 2),
            sprite.getGlobalBounds().getSize()
        );
        bool canMove = true;
        for (const auto& wall : walls)
        {
            if (newBounds.intersects(wall))
            {
                canMove = false;
                break;
            }
        }
        if (canMove)
        {
            setPosition(newPos.x, newPos.y);
        }
        else
        {
            int dir = rand() % 4;
            switch (dir)
            {
            case 0: velocity = { 1,0 }; break;
            case 1: velocity = { -1,0 }; break;
            case 2: velocity = { 0,1 }; break;
            case 3: velocity = { 0,-1 }; break;
            }
            moveClock.restart();
        }
    }
    bool checkDeath(const sf::FloatRect& playerBounds) const
    {
        return playerBounds.intersects(getBounds());
    }
};
vector<Wall> generateMazeDFS()
{
    const int width = MAZE_WIDTH;
    const int height = MAZE_HEIGHT;
    vector<vector<bool>> maze(height, vector<bool>(width, true));
    stack<pair<int, int>> stk;
    maze[1][1] = false;
    stk.push({ 1,1 });
    int dirs[4][2] = { {0,2},{0,-2},{2,0},{-2,0} };
    while (!stk.empty())
    {
        int x = stk.top().first;
        int y = stk.top().second;
        vector<int> available;
        for (int i = 0; i < 4; ++i)
        {
            int nx = x + dirs[i][0];
            int ny = y + dirs[i][1];
            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && maze[ny][nx])
            {
                available.push_back(i);
            }
        }
        if (!available.empty()) {
            int r = available[rand() % available.size()];
            int nx = x + dirs[r][0];
            int ny = y + dirs[r][1];
            maze[y + dirs[r][1] / 2][x + dirs[r][0] / 2] = false;
            maze[ny][nx] = false;
            stk.push({ nx, ny });
        }
        else
        {
            stk.pop();
        }
    }
    vector<Wall> walls;
    for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
            if (maze[y][x]) {
                walls.emplace_back(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            }
        }
    }
    return walls;
}
int main()
{
    srand(time(nullptr));
    setlocale(0, "ru");
    sf::RenderWindow window(sf::VideoMode(MAZE_WIDTH * TILE_SIZE, MAZE_HEIGHT * TILE_SIZE), "Pac-Man Labyrinth");
    window.setFramerateLimit(60);
    vector<Wall> walls = generateMazeDFS();
    Player player((1 * TILE_SIZE) + TILE_SIZE / 2, (1 * TILE_SIZE) + TILE_SIZE / 2);
    Exit door(((MAZE_WIDTH - 2) * TILE_SIZE) + TILE_SIZE / 2, ((MAZE_HEIGHT - 2) * TILE_SIZE) + TILE_SIZE / 2);
    int skX = MAZE_WIDTH / 2;
    int skY = MAZE_HEIGHT / 2;
    while (true) {
        int rx = skX + (rand() % 5 - 2);
        int ry = skY + (rand() % 5 - 2);
        if (rx % 2 == 1 && ry % 2 == 1 && rx > 0 && rx < MAZE_WIDTH - 1 && ry > 0 && ry < MAZE_HEIGHT - 1) 
        {
            if (!(rx == 1 && ry == 1) && !(rx == MAZE_WIDTH - 2 && ry == MAZE_HEIGHT - 2)) {
                skX = rx; skY = ry;
                break;
            }
        }
    }
    Skeleton skeleton((skX * TILE_SIZE) + TILE_SIZE / 2, (skY * TILE_SIZE) + TILE_SIZE / 2);
    sf::Font font;
    if (!font.loadFromFile("arial.ttf"))
    {
        cerr << "Не удалось загрузить шрифт" << endl;
    }
    sf::Text winText("ПОБЕДА! Ты нашёл дверь!", font, 30);
    winText.setFillColor(sf::Color::Green);
    winText.setPosition(140, 240);
    sf::Text loseText("Поражение... Скелет поймал!", font, 30);
    loseText.setFillColor(sf::Color::Red);
    loseText.setPosition(140, 240);
    sf::Text restartText("R - начать заново", font, 20);
    restartText.setFillColor(sf::Color::White);
    restartText.setPosition(280, 340);
    bool gameOver = false;
    bool won = false;
    while (window.isOpen())
    {
        sf::Event event;
        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed) window.close();
            if (event.type == sf::Event::KeyPressed)
            {
                if (event.key.code == sf::Keyboard::Escape) window.close();
                if (gameOver && event.key.code == sf::Keyboard::R)
                {
                    walls = generateMazeDFS();
                    player.setPosition((1 * TILE_SIZE) + TILE_SIZE / 2, (1 * TILE_SIZE) + TILE_SIZE / 2);
                    door.active = true;
                    door.setPosition(((MAZE_WIDTH - 2) * TILE_SIZE) + TILE_SIZE / 2, ((MAZE_HEIGHT - 2) * TILE_SIZE) + TILE_SIZE / 2);
                    skX = MAZE_WIDTH / 2;
                    skY = MAZE_HEIGHT / 2;
                    while (true) {
                        int rx = skX + (rand() % 5 - 2);
                        int ry = skY + (rand() % 5 - 2);
                        if (rx % 2 == 1 && ry % 2 == 1 && rx > 0 && rx < MAZE_WIDTH - 1 && ry > 0 && ry < MAZE_HEIGHT - 1) {
                            if (!(rx == 1 && ry == 1) && !(rx == MAZE_WIDTH - 2 && ry == MAZE_HEIGHT - 2)) {
                                skX = rx; skY = ry;
                                break;
                            }
                        }
                    }
                    skeleton.setPosition((skX * TILE_SIZE) + TILE_SIZE / 2, (skY * TILE_SIZE) + TILE_SIZE / 2);
                    gameOver = false;
                    won = false;
                }
            }
        }
        if (!gameOver)
        {
            float dx = 0, dy = 0;
            if (sf::Keyboard::isKeyPressed(sf::Keyboard::Left))  dx -= 1;
            if (sf::Keyboard::isKeyPressed(sf::Keyboard::Right)) dx += 1;
            if (sf::Keyboard::isKeyPressed(sf::Keyboard::Up))    dy -= 1;
            if (sf::Keyboard::isKeyPressed(sf::Keyboard::Down))  dy += 1;
            vector<sf::FloatRect> wallBounds;
            for (const auto& w : walls) wallBounds.push_back(w.rect);
            player.move(dx, dy, wallBounds);
            skeleton.update(wallBounds);
            if (door.checkWin(player.getBounds()))
            {
                gameOver = true;
                won = true;
            }

            if (skeleton.checkDeath(player.getBounds()))
            {
                gameOver = true;
                won = false;
            }
        }
        window.clear(sf::Color(10, 10, 20));
        for (const auto& wall : walls) 
        {
            sf::RectangleShape rect(sf::Vector2f(wall.rect.width - WALL_THICKNESS, wall.rect.height - WALL_THICKNESS));
            rect.setPosition(wall.rect.left + WALL_THICKNESS / 2, wall.rect.top + WALL_THICKNESS / 2);
            rect.setFillColor(sf::Color(50, 50, 180));
            window.draw(rect);
        }
        if (door.active) door.draw(window);
        skeleton.draw(window);
        player.draw(window);
        if (gameOver) {
            window.draw(won ? winText : loseText);
            window.draw(restartText);
        }
        window.display();
    }
    return 0;
}
