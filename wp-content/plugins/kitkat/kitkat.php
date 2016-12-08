<?php
/**
 * @package KitKat
 */
/*
Plugin Name: KitKat
Description: Test plugin to evaluate skills in PHP and JS/AJAX based on provided code.
Version: 0.0.2
*/
/**
 * last updated: 09/15/2016
 */
// todo: Install a fresh WordPress and use Twenty Sixteen theme. Install this plugin and fix/add all todos ;-)

// don't expose any info if called directly
if ( ! function_exists( 'add_action' ) ) {
	echo 'Hi there! I\'m just a plugin, not much I can do when called directly.';
	exit;
}

// instantiates class
// todo: initiate class KitKat here. Hint: use proper hook!
add_action( 'PROPER HOOK!', [ 'KitKat', 'init' ] );



class KitKat {

	const KITKAT = '0.0.2';
	const KITKAT__MINIMUM_WP_VERSION = '3.2';

	private $KITKAT__PLUGIN_DIR;
	private $KITKAT__PLUGIN_URL;

	public static function init() {
		$class = __CLASS__;
		new $class;
	}


	public function __construct() {

		$this->KITKAT__PLUGIN_DIR = plugin_dir_path( __FILE__ );
		$this->KITKAT__PLUGIN_URL = plugin_dir_url( __FILE__ );

		add_action( 'wp_enqueue_scripts', [ $this, 'kitkat_enqueue_scripts' ] );
		add_filter( 'the_content', [ $this, 'kitkat_insert_button' ] );

		add_action( 'wp_ajax_kitkat_reveal', [ $this, 'kitkat_reveal_emailtextfield' ] );
		add_action( 'wp_ajax_nopriv_kitkat_reveal', [ $this, 'kitkat_reveal_emailtextfield' ] );

		add_action( 'wp_ajax_kitkat_send', [ $this, 'kitkat_validate_send_email' ] );
		add_action( 'wp_ajax_nopriv_kitkat_send', [ $this, 'kitkat_validate_send_email' ] );

	}


	/**
	 * todo: fix visibility property of function.
	 */
	private function kitkat_enqueue_scripts() {
		global $post;

		// todo: add if condition so this code runs on frontend only. Hint: also check if $post is set and that it only runs on pages (check post_type!)
		if ( !is_admin() && is_post()) {

			// adds stylesheet using cache buster.
			wp_enqueue_style( 'kitkat_css', $this->KITKAT__PLUGIN_URL . 'css/kitkat.css', [], date('YmdHis') );

			// todo: make sure the JS gets enqueued in the footer. Hint: none.
			wp_enqueue_script( 'kitkat_js', $this->KITKAT__PLUGIN_URL . 'js/kitkat.js', [ 'jquery' ] );

			// Get the protocol of the current page
			$protocol = isset( $_SERVER['HTTPS'] ) ? 'https://' : 'http://';

			// sets the ajaxurl
			$params = array(
				// gets url to the admin-ajax.php file using admin_url()
				'ajaxurl' => admin_url( 'admin-ajax.php', $protocol ),
			);
			// prints script to page
			wp_localize_script( 'kitkat_js', 'kitkat_params', $params );
		}

	}


	/**
	 * @param $content
	 *
	 * @return string
	 * adds button right below h1 header (page title)
	 */
	public function kitkat_insert_button( $content ) {
		global $post;

		$html = '';

		if ( isset( $post ) && 'page' == $post->post_type ) {
			// creates a nonce for this button
			$nonce_btn = wp_create_nonce( 'kitkat-' . $post->ID );
			$html      = '<div class="kitkat_wrap" data-id="' . $post->ID . '">
				<input type="text" class="kitkat_email" value="" data-nonceemail="" />
				<input type="button" class="kitkat_btn kitkat_active" value="Email this page" data-action="reveal" data-noncebtn="' . $nonce_btn . '" />
				<div class="kitkat_spinner"></div>
				<div class="kitkat_notification"></div>
			</div>';
		}

		return $html . $content;
	}


	public function kitkat_reveal_emailtextfield() {
		// build the response (fallback)
		$arr_response = [
			'status' => 'error'
		];

		// verifies nonce
		if ( isset( $_POST['nonce'] )
		     && isset( $_POST['post_id'] )
		     && wp_verify_nonce( $_POST['nonce'], 'kitkat-' . $_POST['post_id'] )
		) {
			// builds nonce for sending email (email field)
			$nonce = wp_create_nonce( 'kitkat_email-' . absint( $_POST['post_id'] ) );

			// Build the response if successful (overwrite)
			$arr_response = [
				'status' => 'success',
				'data'   => [
					'textforbutton'   => 'Send Now',
					'actionforbutton' => 'send',
					'nonceforemail'   => $nonce
				]
			];
		}

		// whatever the outcome, send response back
		exit( json_encode( $arr_response ) );
	}


	public function kitkat_validate_send_email() {
		// build the response (fallback)
		$arr_response = [
			'status' => 'error'
		];

		// checks for post_id, verifies nonce and email address
		if ( isset( $_POST['nonce'] )
		     && isset( $_POST['post_id'] )
		     && isset( $_POST['email'] )
		     && '' !== $_POST['email']
		     && is_email( $_POST['email'] )
		     // don't check MX record for now: && $this->mxrecordValidate( $_POST['email'] )
		     && wp_verify_nonce( $_POST['nonce'], 'kitkat_email-' . absint( $_POST['post_id'] ) )
		) {
			// --- send email here
			$headers = array(
				'From: test@example.com',
				'Content-Type: text/html; charset=UTF-8',
			);

			$pid     = absint( $_POST['post_id'] );
			$subject = get_the_title( $pid );
			$content = apply_filters( 'the_content', get_post_field( 'post_content', $pid ) );

			// adds the link to page
			$content .= wpautop( esc_html( 'Link to the article: ' ) . '<a href="' . esc_url( get_permalink( $pid ) ) . '">' . esc_html( $subject ) . '</a>' );

			if ( wp_mail( $_POST['email'], $subject, $content, $headers ) ) {
				// email was sent successfully, response with JSON answer
				// Build the response if successful (overwrite)
				$arr_response = [
					'status' => 'success',
					'data'   => [
						'textforbutton'   => 'Email this page',
						'actionforbutton' => 'reveal',
						'nonceforemail'   => ''
					]
				];
			} else {
				// email wasn't sent.
				error_log( 'ERROR sending mail: ' . $GLOBALS['phpmailer']->ErrorInfo );
			}

		}

		// whatever the outcome, send response back
		exit( json_encode( $arr_response ) );
	}


	/**
	 * @param $email
	 *
	 * @return string
	 * checks if MX record exists for email address.
	 */
	private function mxrecordValidate( $email ) {
		$mx_server = '';

		list( $user, $domain ) = explode( '@', $email );

		$arr_dns = dns_get_record( $domain, DNS_MX );

		if ( isset( $arr_dns[0] ) && $arr_dns[0]['host'] == $domain && ! empty( $arr_dns[0]['target'] ) ) {
			$mx_server = $arr_dns[0]['target'];
		}

		return $mx_server;
	}
}
