import React from 'react';
import Layout from '@theme/Layout';
import Link from '@docusaurus/Link';

export default function Home() {
  return (
    <Layout
      title="Stardew Access"
      description="Accessibility mod and toolkit for Stardew Valley."
    >
      <main style={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '70vh',
        padding: '2rem',
        background: 'var(--ifm-background-color)',
      }}>
        <h1 style={{ fontSize: '2.5rem', marginBottom: 12, textAlign: 'center' }}>
          Stardew Access
        </h1>
        <p style={{ fontSize: '1.25rem', maxWidth: 600, textAlign: 'center', marginBottom: 32 }}>
          Accessibility mod and toolkit for Stardew Valley.<br />
          Making Stardew Valley playable for everyone.
        </p>
        <Link
          className="button button--primary button--lg"
          to="/docs/"
          style={{ fontSize: '1.1rem', padding: '0.8em 2.2em' }}
        >
          Get Started
        </Link>
      </main>
    </Layout>
  );
}
