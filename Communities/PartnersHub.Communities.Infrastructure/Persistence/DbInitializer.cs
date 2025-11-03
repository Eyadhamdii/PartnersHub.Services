using PartnersHub.Communities.Domain.Aggregates.Community;

namespace PartnersHub.Communities.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(CommunitiesDbContext context)
    {
        try
        {
            // Clear database
            if (context.Communities.Any())
            {
                context.Communities.RemoveRange(context.Communities);
                context.CommunityFollowers.RemoveRange(context.CommunityFollowers);
                context.CommunityPosts.RemoveRange(context.CommunityPosts);
                await context.SaveChangesAsync();
            }

            // Add sample communities for PIF portfolio companies
            var saudiaCommunity = Community.Create(
                "SAUDIA",
                "Official community for SAUDIA partners and stakeholders to collaborate and share insights about aviation industry",
                "https://example.com/images/saudia.jpg"
            );

            var neomCommunity = Community.Create(
                "NEOM Innovation Community",
                "A space for NEOM partners to discuss sustainable development and future technologies",
                "https://example.com/images/neom.jpg"
            );

            var acwaPowerCommunity = Community.Create(
                "ACWA Power Renewable Energy",
                "Connecting ACWA Power partners in renewable energy and water desalination projects",
                "https://example.com/images/acwa-power.jpg"
            );

            var maaCommunity = Community.Create(
                "Ma'aden Mining Hub",
                "Discussion forum for Ma'aden partners in mining and mineral development",
                "https://example.com/images/maaden.jpg"
            );

            var redSeaCommunity = Community.Create(
                "Red Sea Global Development",
                "Community for partners involved in Red Sea tourism development projects",
                "https://example.com/images/red-sea.jpg"
            );

            var communities = new[] { saudiaCommunity, neomCommunity, acwaPowerCommunity, maaCommunity, redSeaCommunity };
            await context.Communities.AddRangeAsync(communities.AsEnumerable());
            await context.SaveChangesAsync();

            // Create sample users (representing different partner companies)
            var userIds = new Dictionary<string, Guid>
            {
                ["TechConsultant"] = Guid.NewGuid(),
                ["Contractor"] = Guid.NewGuid(),
                ["Supplier"] = Guid.NewGuid(),
                ["Investor"] = Guid.NewGuid(),
                ["Advisor"] = Guid.NewGuid()
            };

            // Add sample posts for each community
            await AddPostsToCommunitiesAsync(communities, userIds, context);

            // Add followers to demonstrate engagement
            await AddFollowersToCommunitiesAsync(communities, userIds, context);
        }
        catch (Exception ex)
        {
            // Log the error - in a real application, use proper logging
            System.Diagnostics.Debug.WriteLine($"Error initializing database: {ex.Message}");
            throw; // Re-throw to ensure the error is handled by the caller
        }
    }

    private static async Task AddPostsToCommunitiesAsync(
        IEnumerable<Community> communities,
        Dictionary<string, Guid> userIds,
        CommunitiesDbContext context)
    {
        foreach (var community in communities)
        {
            switch (community.Name.Value)
            {
                case "SAUDIA":
                    community.AddPost("Introducing new partnership opportunities in SAUDIA's digital transformation initiative", userIds["TechConsultant"]);
                    community.AddPost("Seeking innovative solutions for sustainable aviation fuel implementation", userIds["Supplier"]);
                    break;

                case "NEOM Innovation Community":
                    community.AddPost("Updates on The Line project: New technology partnership opportunities", userIds["Contractor"]);
                    community.AddPost("Innovation challenge: Smart city solutions for NEOM sectors", userIds["Advisor"]);
                    community.AddPost("Upcoming partner workshop on NEOM's renewable energy infrastructure", userIds["Supplier"]);
                    break;

                case "ACWA Power Renewable Energy":
                    community.AddPost("Partner collaboration opportunity: New solar power plant project in Saudi Arabia", userIds["Investor"]);
                    community.AddPost("Success story: Implementation of smart grid solutions with our technology partners", userIds["TechConsultant"]);
                    break;

                case "Ma'aden Mining Hub":
                    community.AddPost("New mineral exploration project seeking technology partners", userIds["Advisor"]);
                    community.AddPost("Environmental sustainability practices in mining operations - Partner showcase", userIds["Contractor"]);
                    break;

                case "Red Sea Global Development":
                    community.AddPost("Luxury tourism development: New partnership opportunities for hospitality providers", userIds["Investor"]);
                    community.AddPost("Sustainable tourism infrastructure: Seeking innovative solutions from partners", userIds["Supplier"]);
                    break;
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task AddFollowersToCommunitiesAsync(
        IEnumerable<Community> communities,
        Dictionary<string, Guid> userIds,
        CommunitiesDbContext context)
    {
        foreach (var community in communities)
        {
            switch (community.Name.Value)
            {
                case "SAUDIA":
                    community.AddFollower(userIds["TechConsultant"]);
                    community.AddFollower(userIds["Supplier"]);
                    break;

                case "NEOM Innovation Community":
                    community.AddFollower(userIds["Contractor"]);
                    community.AddFollower(userIds["TechConsultant"]);
                    community.AddFollower(userIds["Advisor"]);
                    break;

                case "ACWA Power Renewable Energy":
                    community.AddFollower(userIds["Investor"]);
                    community.AddFollower(userIds["Supplier"]);
                    break;

                case "Ma'aden Mining Hub":
                    community.AddFollower(userIds["Contractor"]);
                    community.AddFollower(userIds["Advisor"]);
                    break;

                case "Red Sea Global Development":
                    community.AddFollower(userIds["Investor"]);
                    community.AddFollower(userIds["TechConsultant"]);
                    break;
            }
        }

        await context.SaveChangesAsync();
    }
}