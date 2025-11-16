import { themes as prismThemes } from "prism-react-renderer";
import type { Config } from "@docusaurus/types";
import type * as Preset from "@docusaurus/preset-classic";

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

const config: Config = {
	title: "Stardew Access",
	tagline: "Accessibility for Stardew Valley",
	favicon: "img/favicon.ico",

	// Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
	future: {
		v4: true, // Improve compatibility with the upcoming Docusaurus v4
	},

	// Set the production url of your site here
	url: "https://stardew-access.github.io",
	// Set the /<baseUrl>/ pathname under which your site is served
	// For GitHub pages deployment, it is often '/<projectName>/'
	baseUrl: "/stardew-access/",

	// GitHub pages deployment config.
	// If you aren't using GitHub pages, you don't need these.
	organizationName: "stardew-access", // Your GitHub username
	projectName: "stardew-access", // Your repo name

	onBrokenLinks: "throw",

	// Even if you don't use internationalization, you can use this field to set
	// useful metadata like html lang. For example, if your site is Chinese, you
	// may want to replace "en" with "zh-Hans".
	i18n: {
		defaultLocale: "en",
		locales: ["en", "es"], // English and Spanish
		localeConfigs: {
			en: {
				label: "English",
			},
			es: {
				label: "Español",
			},
		},
	},

	presets: [
		[
			"classic",
			{
				docs: {
					id: "default",
					path: "./docs",
					routeBasePath: "docs",
					sidebarPath: require.resolve("./sidebars.ts"),
					editUrl:
						"https://github.com/stardew-access/stardew-access/website/docs/",
				},
				theme: {
					customCss: "./src/css/custom.css",
				},
			} satisfies Preset.Options,
		],
	],

	plugins: [
		[
			"@docusaurus/plugin-content-docs",
			{
				id: "guides",
				path: "./guides",
				routeBasePath: "guides",
				sidebarPath: require.resolve("./sidebarsGuides.ts"),
				editUrl: "https://github.com/stardew-access/stardew-access/website/guides/",
			},
		],
	],

	themeConfig: {
		// Replace with your project's social card
		image: "img/stardew-access-social-card.jpg", // Replace with your own image if available
		colorMode: {
			respectPrefersColorScheme: true,
		},
		navbar: {
			title: "Stardew Access",
			logo: {
				alt: "Stardew Access Logo",
				src: "img/logo.svg", // Replace with your own logo if available
			},
			items: [
				{
					type: "docSidebar",
					sidebarId: "docsSidebar",
					position: "left",
					label: "Docs",
				},
				{
					type: "docSidebar",
					sidebarId: "guidesSidebar",
					docsPluginId: "guides",
					position: "left",
					label: "Guides",
				},
				{
					href: "https://github.com/stardew-access/stardew-access",
					label: "GitHub",
					position: "right",
				},
			],
		},
		footer: {
			style: "dark",
			links: [
				{
					title: "Docs",
					items: [
						{
							label: "Home",
							to: "/docs/",
						},
						{
							label: "Features",
							to: "/docs/features",
						},
						{
							label: "Setup",
							to: "/docs/setup",
						},
					],
				},
				{
					title: "Community",
					items: [
						{
							label: "GitHub Discussions",
							href: "https://github.com/stardew-access/stardew-access/discussions",
						},
					],
				},
				{
					title: "More",
					items: [
						{
							label: "GitHub",
							href: "https://github.com/stardew-access/stardew-access",
						},
					],
				},
			],
			copyright: `Copyright \u00a9 ${new Date().getFullYear()} Stardew Access. Built with Docusaurus.`,
		},
		prism: {
			theme: prismThemes.github,
			darkTheme: prismThemes.dracula,
		},
	} satisfies Preset.ThemeConfig,
};

export default config;
