<template>
	<div class="paramdetails">
		<span>The parameter value that was passed to me is: {{ $route.params.id }}</span>
	</div>
</template>
	
<script>
	export default {
		name: 'paramdetails'
	}
</script>