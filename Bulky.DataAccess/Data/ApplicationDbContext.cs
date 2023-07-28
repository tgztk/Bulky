﻿using Bulky.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
	public DbSet<Category> Categories { get; set; }

	public DbSet<Product> Products { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Category>().HasData(
			new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
			new Category { Id = 2, Name = "Sci-Fi", DisplayOrder = 2 },
			new Category { Id = 3, Name = "History", DisplayOrder = 3 }
			);

		modelBuilder.Entity<Product>().HasData(
			new Product
			{
				Id = 1,
				Title = "22/11/63",
				Author = "Stephen King",
				Description = "11/22/63 is a novel by Stephen King about a time traveller who attempts to prevent the assassination of United States President John F. Kennedy, which occurred on November 22, 1963.",
				ISBN = Guid.NewGuid().ToString()[..12].ToUpper(),
				ListPrice = 100,
				PriceFifty = 70,
				PriceHundredOrMore = 60,
				CategoryId = 1,
				ImageUrl = "\\images\\product\\initial\\22-11-63.jpg"
			},
			new Product
			{
				Id = 2,
				Title = "Alamut",
				Author = "Vladimir Bartol",
				Description = "Alamut is a novel by Vladimir Bartol, first published in 1938 in Slovenian, dealing with the story of Hassan-i Sabbah and the Hashshashin, and named after their Alamut fortress. The maxim of the novel is \"Nothing is an absolute reality; all is permitted\". This book was one of the inspirations for the video game series Assassin's Creed.",
				ISBN = Guid.NewGuid().ToString()[..12].ToUpper(),
				ListPrice = 150,
				PriceFifty = 80,
				PriceHundredOrMore = 70,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\alamut.jpg"
			},
			new Product
			{
				Id = 3,
				Title = "Fortune of Time",
				Author = "Billy Spark",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "SWD9999001",
				ListPrice = 99,
				PriceFifty = 85,
				PriceHundredOrMore = 80,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\fortune of time.jpg"
			},
			new Product
			{
				Id = 4,
				Title = "Dark Skies",
				Author = "Nancy Hoover",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "CAW777777701",
				ListPrice = 40,
				PriceFifty = 25,
				PriceHundredOrMore = 20,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\dark skies.jpg"
			},
			new Product
			{
				Id = 5,
				Title = "Vanish in the Sunset",
				Author = "Julian Button",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "RITO5555501",
				ListPrice = 55,
				PriceFifty = 40,
				PriceHundredOrMore = 35,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\vanish in the sunset.jpg"
			},
			new Product
			{
				Id = 6,
				Title = "Cotton Candy",
				Author = "Abby Muscles",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "WS3333333301",
				ListPrice = 70,
				PriceFifty = 60,
				PriceHundredOrMore = 55,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\cotton candy.jpg"
			},
			new Product
			{
				Id = 7,
				Title = "Rock in the Ocean",
				Author = "Ron Parker",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "SOTJ1111111101",
				ListPrice = 30,
				PriceFifty = 25,
				PriceHundredOrMore = 20,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\rock in the ocean.jpg"
			},
			new Product
			{
				Id = 8,
				Title = "Leaves and Wonders",
				Author = "Laura Phantom",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
				ISBN = "FOT000000001",
				ListPrice = 25,
				PriceFifty = 22,
				PriceHundredOrMore = 20,
				CategoryId = 2,
				ImageUrl = "\\images\\product\\initial\\leaves and wonders.jpg"
			}
		);
	}
}
