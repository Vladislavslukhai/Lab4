using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetworkSimulation
{
    struct Comment
    {
        public string AuthorNickname;
        public string Text;
        public DateTime CreatedAt;

        public Comment(string authorNickname, string text)
        {
            AuthorNickname = authorNickname;
            Text = text;
            CreatedAt = DateTime.Now;
        }
    }

    class Post
    {
        public int Id { get; }
        public DateTime CreatedAt { get; }
        public string Text { get; }
        public int Likes { get; private set; }
        public List<Comment> Comments { get; }

        public Post(int id, string text, DateTime createdAt)
        {
            Id = id;
            Text = text;
            CreatedAt = createdAt;
            Likes = 0;
            Comments = new List<Comment>();
        }

        public void AddLike()
        {
            Likes++;
        }

        public void AddComment(string authorNickname, string text)
        {
            Comments.Add(new Comment(authorNickname, text));
        }

        public void PrintFullInfo()
        {
            Console.WriteLine($"[ID: {Id}] Дата: {CreatedAt:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Текст: {Text}");
            Console.WriteLine($"Лайків: {Likes} | Коментарів: {Comments.Count}");

            if (Comments.Count > 0)
            {
                Console.WriteLine("Коментарі:");
                for (int i = 0; i < Comments.Count; i++)
                {
                    Console.WriteLine($"   {Comments[i].AuthorNickname} ({Comments[i].CreatedAt:HH:mm}): {Comments[i].Text}");
                }
            }
            Console.WriteLine("-------------------");
        }
    }

    class User
    {
        public string Name { get; }
        public string Email { get; }
        public string Nickname { get; }
        public List<Post> Posts { get; }
        private int _nextPostId;

        public User(string name, string email, string nickname)
        {
            Name = name;
            Email = email;
            Nickname = nickname;
            Posts = new List<Post>();
            _nextPostId = 1;
        }

        public void CreatePost(string text)
        {
            Posts.Add(new Post(_nextPostId++, text, DateTime.Now));
        }

        public void CreatePostWithDate(string text, DateTime date)
        {
            Posts.Add(new Post(_nextPostId++, text, date));
        }

        public bool DeletePost(int postId)
        {
            for (int i = 0; i < Posts.Count; i++)
            {
                if (Posts[i].Id == postId)
                {
                    Posts.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public Post FindPostById(int postId)
        {
            for (int i = 0; i < Posts.Count; i++)
            {
                if (Posts[i].Id == postId)
                    return Posts[i];
            }
            return null;
        }

        public void ShowLastFivePostsOf(User otherUser)
        {
            Console.WriteLine($"\nОстанні пости користувача {otherUser.Name} ({otherUser.Nickname}):");
            Console.WriteLine("-------------------");

            if (otherUser.Posts.Count == 0)
            {
                Console.WriteLine("У користувача ще немає постів.");
                return;
            }

            int startIndex = Math.Max(0, otherUser.Posts.Count - 5);
            for (int i = startIndex; i < otherUser.Posts.Count; i++)
            {
                otherUser.Posts[i].PrintFullInfo();
            }
        }

        public bool LikePostOf(User otherUser, int postId)
        {
            Post post = otherUser.FindPostById(postId);
            if (post == null)
                return false;

            post.AddLike();
            return true;
        }

        public bool CommentPostOf(User otherUser, int postId, string commentText)
        {
            Post post = otherUser.FindPostById(postId);
            if (post == null)
                return false;

            post.AddComment(Nickname, commentText);
            return true;
        }
    }

    class SocialNetwork
    {
        public List<User> Users { get; }

        public SocialNetwork()
        {
            Users = new List<User>();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }

        public User FindUserByNickname(string nickname)
        {
            if (!nickname.StartsWith("@"))
                nickname = "@" + nickname;

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
                    return Users[i];
            }
            return null;
        }

        public void PrintAllUsers()
        {
            Console.WriteLine("Користувачі в системі:");
            for (int i = 0; i < Users.Count; i++)
            {
                Console.WriteLine($"- {Users[i].Name} ({Users[i].Nickname}) | {Users[i].Email} | Постів: {Users[i].Posts.Count}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            SocialNetwork network = new SocialNetwork();

            User currentUser = new User("Влад", "vlad@example.com", "@vlad_dev");
            User olena = new User("Олена", "olena@example.com", "@olena_art");
            User maksym = new User("Максим", "maks@example.com", "@maks_tech");

            olena.CreatePostWithDate("Пост 1: Почала вивчати C#", DateTime.Now.AddDays(-6));
            olena.CreatePostWithDate("Пост 2: Сьогодні чудова погода", DateTime.Now.AddDays(-5));
            olena.CreatePostWithDate("Пост 3: Малюю новий арт", DateTime.Now.AddDays(-4));
            olena.CreatePostWithDate("Пост 4: П'ю ранкову каву", DateTime.Now.AddDays(-3));
            olena.CreatePostWithDate("Пост 5: Готуюсь до пар", DateTime.Now.AddDays(-2));
            olena.CreatePostWithDate("Пост 6: Щойно закрила лабораторну!", DateTime.Now.AddDays(-1));
            olena.LikePostOf(olena, 6);
            olena.CommentPostOf(olena, 6, "Вітаю з успішним захистом!");

            maksym.CreatePostWithDate("Зібрав новий ПК", DateTime.Now.AddDays(-2));
            maksym.CreatePostWithDate("Хто грає ввечері?", DateTime.Now.AddHours(-5));

            currentUser.CreatePost("Мій перший пост у цій соцмережі!");

            network.AddUser(currentUser);
            network.AddUser(olena);
            network.AddUser(maksym);

            while (true)
            {
                Console.WriteLine($"\nВи увійшли як: {currentUser.Name} ({currentUser.Nickname})");
                Console.WriteLine("1 - Переглянути список користувачів");
                Console.WriteLine("2 - Отримати останні 5 постів користувача");
                Console.WriteLine("3 - Поставити лайк посту іншого користувача");
                Console.WriteLine("4 - Прокоментувати пост іншого користувача");
                Console.WriteLine("5 - Створити новий пост");
                Console.WriteLine("6 - Видалити свій пост");
                Console.WriteLine("7 - Переглянути мої пости");
                Console.WriteLine("0 - Вихід");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        network.PrintAllUsers();
                        break;

                    case "2":
                        Console.Write("Введіть нікнейм користувача (наприклад, @olena_art): ");
                        User targetView = network.FindUserByNickname(Console.ReadLine());
                        if (targetView != null)
                            currentUser.ShowLastFivePostsOf(targetView);
                        else
                            Console.WriteLine("Користувача не знайдено.");
                        break;

                    case "3":
                        Console.Write("Введіть нікнейм автора поста: ");
                        User targetLike = network.FindUserByNickname(Console.ReadLine());
                        if (targetLike == null)
                        {
                            Console.WriteLine("Користувача не знайдено.");
                            break;
                        }
                        Console.Write("Введіть ID поста: ");
                        int likePostId = int.Parse(Console.ReadLine());
                        if (currentUser.LikePostOf(targetLike, likePostId))
                            Console.WriteLine("Лайк поставлено!");
                        else
                            Console.WriteLine("Пост із таким ID не знайдено.");
                        break;

                    case "4":
                        Console.Write("Введіть нікнейм автора поста: ");
                        User targetComment = network.FindUserByNickname(Console.ReadLine());
                        if (targetComment == null)
                        {
                            Console.WriteLine("Користувача не знайдено.");
                            break;
                        }
                        Console.Write("Введіть ID поста: ");
                        int commentPostId = int.Parse(Console.ReadLine());
                        Console.Write("Введіть текст коментаря: ");
                        string commentText = Console.ReadLine();
                        if (currentUser.CommentPostOf(targetComment, commentPostId, commentText))
                            Console.WriteLine("Коментар додано!");
                        else
                            Console.WriteLine("Пост із таким ID не знайдено.");
                        break;

                    case "5":
                        Console.Write("Введіть текст нового поста: ");
                        string newPostText = Console.ReadLine();
                        currentUser.CreatePost(newPostText);
                        Console.WriteLine("Пост успішно опубліковано!");
                        break;

                    case "6":
                        Console.Write("Введіть ID свого поста для видалення: ");
                        int deletePostId = int.Parse(Console.ReadLine());
                        if (currentUser.DeletePost(deletePostId))
                            Console.WriteLine("Пост видалено.");
                        else
                            Console.WriteLine("Пост із таким ID не знайдено.");
                        break;

                    case "7":
                        currentUser.ShowLastFivePostsOf(currentUser);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }
            }
        }
    }
}