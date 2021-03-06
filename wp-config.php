<?php
/**
 * The base configuration for WordPress
 *
 * The wp-config.php creation script uses this file during the
 * installation. You don't have to use the web site, you can
 * copy this file to "wp-config.php" and fill in the values.
 *
 * This file contains the following configurations:
 *
 * * MySQL settings
 * * Secret keys
 * * Database table prefix
 * * ABSPATH
 *
 * @link https://codex.wordpress.org/Editing_wp-config.php
 *
 * @package WordPress
 */

// ** MySQL settings - You can get this info from your web host ** //
/** The name of the database for WordPress */
define('DB_NAME', 'database_staging');
  
/** MySQL database username */
define('DB_USER', 'db_patil');

/** MySQL database password */
define('DB_PASSWORD', 'admin12345');
 
/** MySQL hostname */
define('DB_HOST', 'localhost');

/** Database Charset to use in creating database tables. */
define('DB_CHARSET', 'utf8mb4');

/** The Database Collate type. Don't change this if in doubt. */
define('DB_COLLATE', '');

/**#@+
 * Authentication Unique Keys and Salts.
 *
 * Change these to different unique phrases!
 * You can generate these using the {@link https://api.wordpress.org/secret-key/1.1/salt/ WordPress.org secret-key service}
 * You can change these at any point in time to invalidate all existing cookies. This will force all users to have to log in again.
 *
 * @since 2.6.0
 */
define('AUTH_KEY',         'cZP4$<}P??O3ZWv-c6*$INapbs:>F)!jng{1Wq `I|sKZ,/IX*<ekqjg@])4H(hh');
define('SECURE_AUTH_KEY',  'A#u`EeDX|I7<=!-3l#gr]qfRW{n3)!:l=p`R,I6}QvegQPK.=&LT0g;Rh9,64H4j');
define('LOGGED_IN_KEY',    'n/pLH[2jr4c<xN&kP#T9$u]&1|TLW<~z{{{E=xQlZy@Ewh|!qmD|F<=xttXofLiD');
define('NONCE_KEY',        'V|(@D#K%gl}raNZnQ{7lve?H?1]77;D-{T@+k@SCns6(zgzLWMa)mB_L[pgGM06<');
define('AUTH_SALT',        'J;2lqOTg?Zt.#Kjkt~Z=#9XPm;$GZKV}b6A]35Kdsd0QI{}<j8Ws?0h/}F:lbDnI');
define('SECURE_AUTH_SALT', '%@`]MEwbXK<.&pNWE;.>5L+-?+%b[/r#I<;l{4JDiV)NBopAKpl;7Ybp|3.@/?fD');
define('LOGGED_IN_SALT',   'm?A/SIX&t;l!wwwTY^HBh>&SD)au$MW:2J*x^mHH%&0&B^M(<qP_J bt|!b/i |i');
define('NONCE_SALT',       '%/f@X372kE+^xd2lJP)cd?FFG](9Ll6><h&Lx}S/gjM;!9X+L6)4nsykh*. @wLA');

/**#@-*/

/**
 * WordPress Database Table prefix.
 *
 * You can have multiple installations in one database if you give each
 * a unique prefix. Only numbers, letters, and underscores please!
 */
$table_prefix  = 'wp_';

/**
 * For developers: WordPress debugging mode.
 *
 * Change this to true to enable the display of notices during development.
 * It is strongly recommended that plugin and theme developers use WP_DEBUG
 * in their development environments.
 *
 * For information on other constants that can be used for debugging,
 * visit the Codex.
 *
 * @link https://codex.wordpress.org/Debugging_in_WordPress
 */
define('WP_DEBUG', false);

/* That's all, stop editing! Happy blogging. */

/** Absolute path to the WordPress directory. */
if ( !defined('ABSPATH') )
	define('ABSPATH', dirname(__FILE__) . '/');

/** Sets up WordPress vars and included files. */
require_once(ABSPATH . 'wp-settings.php');
