<template>
	<div class="tree-menu">
		<div class="label-wrapper" @click="toggleChildren">
			<div :style="indent" :class="labelClasses">
				<i v-if="node.isFolder" class="fa" :class="iconClasses"></i>
				<i v-if="!node.isFolder" class="fa fa-key"></i>
				{{ label }}
			</div>
		</div>
		<tree-menu 
			v-if="showChildren"
			v-for="node in nodes" :key="node.id"
			:nodes="node.nodes" 
			:node="node"
			:label="node.label"
			:depth="depth + 1">
		</tree-menu>
	</div>
</template>
<script>
	import Vue from 'vue'
	
	export default { 
		name: 'tree-menu',
		props: [ 'label', 'nodes', 'depth','node','onExpand' ],
		data() {
			return { 
				showChildren: false 
			}
		},
		computed: {
		    iconClasses() {
		      return {
		        'fa-plus-square-o': !this.showChildren,
		        'fa-minus-square-o': this.showChildren
		      }
		    },
		    labelClasses() {
		      return { 'has-children': this.nodes }
		    },
			indent() {
				return { transform: `translate(${this.depth * 50}px)` }
			}
		},
		methods: {
			
			toggleChildren() {
				if(this.node.blocked())return;
				if(this.node.isFolder){
					this.showChildren = !this.showChildren;
					if(this.node.onExpand && this.showChildren){
						while(this.node.nodes.length > 0) {
						    this.node.nodes.pop();
						}
						this.node.onExpand(this.node);
					}
				}
				if(this.node.onClick){
					this.node.onClick(this.node);
				}
			}
		}
	}
</script>